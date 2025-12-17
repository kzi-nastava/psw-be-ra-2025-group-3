using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Execution;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;

    private readonly ITourPurchaseTokenRepository _tokenRepository;

    private readonly IKeyPointRepository _keyPointRepository;
    private readonly IShoppingCartService _shoppingCartService;

    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,

        ITourPurchaseTokenRepository tokenRepository,

        IKeyPointRepository keyPointRepository,
        IShoppingCartService shoppingCartService,

        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;

        _tokenRepository = tokenRepository;

        _keyPointRepository = keyPointRepository;
        _shoppingCartService = shoppingCartService;

        _mapper = mapper;
    }

    public TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId)
    {
        
        // Proveri da li turista već ima bilo kakvu aktivnu turu
        var existingActiveExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (existingActiveExecution != null)
        {
            throw new InvalidOperationException(
                $"Cannot start: You already have an active tour (ID: {existingActiveExecution.TourId}). " +
                "Please complete or abandon it first."
            );
        }

        // Provera postojanja ture
        var tour = _tourRepository.GetByIdWithKeyPoints(dto.TourId);
        if (tour == null)
            throw new NotFoundException($"Tour with id {dto.TourId} not found.");

        // Provera statusa - Published ili Archived
        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Cannot start: Tour is not available.");

        // Validacija kupovine
        //var hasPurchased = _shoppingCartService.HasPurchasedTour(touristId, dto.TourId);
        //if (!hasPurchased)
        //    throw new InvalidOperationException("Cannot start: Tour must be purchased first.");


        // Provera broja ključnih tačaka
        var keyPointCount = tour.KeyPoints?.Count ?? 0;
        if (keyPointCount < 2)
            throw new InvalidOperationException("Cannot start: Tour must have at least 2 key points.");

        //if (_executionRepository.HasActiveSession(touristId, dto.TourId))
            //throw new InvalidOperationException("Cannot start: You already have an active session for this tour.");

        // Kreiranje nove sesije
        var execution = new TourExecution(
            touristId,
            dto.TourId,
            dto.StartLatitude,
            dto.StartLongitude
        );

        var created = _executionRepository.Create(execution);
        return _mapper.Map<TourExecutionDto>(created);
    }

    public TourExecutionDto? GetActiveTourExecution(long touristId)
    {
        var activeExecution = _executionRepository.GetActiveByTouristId(touristId);

        if (activeExecution == null)
            return null;

        return _mapper.Map<TourExecutionDto>(activeExecution);
    }

    //task2
    public LocationCheckResultDto CheckLocationProgress(LocationCheckDto dto, long touristId)
    {
        // Učitaj aktivnu TourExecution sesiju
        var execution = _executionRepository.GetActiveExecution(touristId, dto.TourId);

        if (execution == null)
            throw new InvalidOperationException("No active tour session found.");

        // Učitaj KeyPoints ture
        var tour = _tourRepository.GetByIdWithKeyPoints(execution.TourId);

        if (tour == null || tour.KeyPoints == null || !tour.KeyPoints.Any())
            throw new InvalidOperationException("Tour or KeyPoints not found.");

        // Pozovi agregat metodu
        bool keyPointCompleted = execution.CheckLocationProgress(
            dto.CurrentLatitude,
            dto.CurrentLongitude,
            tour.KeyPoints.ToList()
        );

       
        _executionRepository.Update(execution);

        // Vrati rezultat
        return new LocationCheckResultDto
        {
            KeyPointCompleted = keyPointCompleted,
            CompletedKeyPointId = keyPointCompleted && execution.CompletedKeyPoints.Any()
                ? execution.CompletedKeyPoints.Last().KeyPointId
                : null,
            LastActivity = execution.LastActivity,
            TotalCompletedKeyPoints = execution.CompletedKeyPoints.Count
        };
    }


    public bool CanStartTour(long touristId, long tourId)
    {
        // pozivamo AccessService preko repoa
        var tokens = _tokenRepository.GetByTouristId(touristId);

        return tokens.Any(t => t.TourId == tourId);
}
    public TourExecutionDto CompleteTour(long touristId)
    {
        var activeExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (activeExecution == null)
            throw new NotFoundException("No active tour session found.");

        // UČITAJ TURU SA KEY POINTS
        var tour = _tourRepository.GetByIdWithKeyPoints(activeExecution.TourId);
        if (tour == null || tour.KeyPoints == null || !tour.KeyPoints.Any())
            throw new InvalidOperationException("Tour or KeyPoints not found.");

        // DA LI SU SVE KEY POINTS KOMPLETIRANE
        var totalKeyPoints = tour.KeyPoints.Count;
        var completedKeyPoints = activeExecution.CompletedKeyPoints?.Count ?? 0;

        if (completedKeyPoints < totalKeyPoints)
        {
            throw new InvalidOperationException(
                $"Cannot complete tour: You have completed {completedKeyPoints}/{totalKeyPoints} key points. " +
                "Complete all key points before finishing the tour."
            );
        }

        //  AKO SU SVE KOMPLETIRANE, DOZVOLI COMPLETE
        activeExecution.Complete();
        var updated = _executionRepository.Update(activeExecution);
        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionDto AbandonTour(long touristId)
    {
        var activeExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (activeExecution == null)
            throw new NotFoundException("No active tour session found.");

        activeExecution.Abandon();
        var updated = _executionRepository.Update(activeExecution);
        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionWithNextKeyPointDto? GetActiveWithNextKeyPoint(long touristId)
    {
        var activeExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (activeExecution == null)
            return null;

        var tour = _tourRepository.GetByIdWithKeyPoints(activeExecution.TourId);
        if (tour == null || tour.KeyPoints == null || !tour.KeyPoints.Any())
            return null;

        var firstKeyPoint = tour.KeyPoints.OrderBy(kp => kp.Id).First();

        var result = new TourExecutionWithNextKeyPointDto
        {
            Id = activeExecution.Id,
            TouristId = activeExecution.TouristId,
            TourId = activeExecution.TourId,
            StartTime = activeExecution.StartTime,
            Status = (int)activeExecution.Status,
            StartLatitude = activeExecution.StartLatitude,
            StartLongitude = activeExecution.StartLongitude,
            CompletionTime = activeExecution.CompletionTime,
            AbandonTime = activeExecution.AbandonTime,
            NextKeyPoint = _mapper.Map<KeyPointDto>(firstKeyPoint),
            DistanceToNextKeyPoint = CalculateDistance(
                activeExecution.StartLatitude,
                activeExecution.StartLongitude,
                firstKeyPoint.Latitude,
                firstKeyPoint.Longitude)
        };

        return result;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var earthRadiusMeters = 6371e3;

        // Konverzija u radijane
        var lat1Rad = lat1 * Math.PI / 180;
        var lat2Rad = lat2 * Math.PI / 180;
        var latDiff = (lat2 - lat1) * Math.PI / 180;
        var lonDiff = (lon2 - lon1) * Math.PI / 180;

        // Haversine formula
        var a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusMeters * c;

    }
}
