using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Core.UseCases.Execution;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;

    //private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IInternalTokenService _tokenService;

    private readonly IKeyPointRepository _keyPointRepository;

    //private readonly IShoppingCartService _shoppingCartService;
    private readonly IInternalShoppingCartService _shoppingCartService;

    private readonly IInternalXpEventService _xpEventService;
    private readonly IInternalNotificationService _notificationService;
    private readonly IInternalAchievementService _achievementService;

    private readonly IGroupTourSessionCleanup _groupTourSessionCleanup;
    private readonly ITourService _tourService;
    private readonly ITouristTourService _touristTourService;

    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,

        //ITourPurchaseTokenRepository tokenRepository,
        IInternalTokenService tokenService,

        IKeyPointRepository keyPointRepository,
        
        //IShoppingCartService shoppingCartService,
        IInternalShoppingCartService shoppingCartService,

        IInternalXpEventService xpEventService,


        IInternalNotificationService notificationService,
        IInternalAchievementService achievementService,

        IGroupTourSessionCleanup groupTourSessionCleanup,
        ITourService tourService,


        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;

        //_tokenRepository = tokenRepository
        _tokenService = tokenService;

        _keyPointRepository = keyPointRepository;
        
        //_shoppingCartService = shoppingCartService;
        _shoppingCartService = shoppingCartService;

        _notificationService = notificationService;
        _achievementService = achievementService;

        _xpEventService = xpEventService;

        _groupTourSessionCleanup = groupTourSessionCleanup;
        _tourService = tourService;

        _mapper = mapper;
    }

    public TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId)
    {
        
        // Proveri da li turista već ima bilo kakvu aktivnu turu
        // Ako ima, automatski je napusti pre pokretanja nove
        var existingActiveExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (existingActiveExecution != null)
        {
            // Automatski napusti staru aktivnu turu
            if (existingActiveExecution.GroupSessionId != null)
                _groupTourSessionCleanup.HandleAbandon(existingActiveExecution.Id);
            
            existingActiveExecution.Abandon();
            _executionRepository.Update(existingActiveExecution);
        }

        // Provera postojanja ture
        var tour = _tourRepository.GetByIdWithKeyPoints(dto.TourId);
        if (tour == null)
            throw new NotFoundException($"Tour with id {dto.TourId} not found.");

        // Provera statusa - Published ili Archived
        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Cannot start: Tour is not available.");

        // Validacija kupovine
        var hasPurchased = _tokenService.GetTokens(touristId)
            .Any(t => t.TourId == dto.TourId);
        if (!hasPurchased)
            throw new InvalidOperationException("Cannot start: Tour must be purchased first.");

        // Provera broja ključnih tačaka
        var keyPointCount = tour.KeyPoints?.Count ?? 0;
        if (keyPointCount < 2)
            throw new InvalidOperationException("Cannot start: Tour must have at least 2 key points.");

        if (_executionRepository.HasActiveSession(touristId, dto.TourId))
            throw new InvalidOperationException("Cannot start: You already have an active session for this tour.");

        // Kreiranje nove sesije
        //touristId,
        //dto.TourId,
        //dto.StartLatitude,
        //dto.StartLongitude,
        //dto.GroupSessionId
        var execution = new TourExecution(
            touristId,
            dto.TourId,
            dto.StartLatitude,
            dto.StartLongitude,
            dto.GroupSessionId            
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

    public TourDto? GetActiveTourByTouristId(long touristId)
    {
        var activeTourExecution = GetActiveTourExecution(touristId);

        if (activeTourExecution == null)
            return null;

        var activeTour = _tourService.GetById(activeTourExecution.TourId);
        if (activeTour == null)
            throw new KeyNotFoundException("No tour found for the active tour execution.");

        return activeTour;
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

        // Pozovi agregat metodu - proverava distancu do SVIH nekompletiranih key points
        // Turista može da otključi bilo koju key point koja je blizu (200 metara)
        // Nema obaveznog redosleda - može prvo obići key point 4 ako je blizu, pa onda key point 1
        bool keyPointCompleted = execution.CheckLocationProgress(
            dto.CurrentLatitude,
            dto.CurrentLongitude,
            tour.KeyPoints.ToList()
        );

       
        _executionRepository.Update(execution);

        // Pronađi sledeću nekompletiranu key point (za prikaz korisniku)
        var finalNextRequiredKeyPoint = execution.GetNextRequiredKeyPoint(tour.KeyPoints.ToList());

        // Formiraj poruku o otključavanju
        string? message = null;
        if (!keyPointCompleted && finalNextRequiredKeyPoint != null)
        {
            message = $"Pronađite i obiđite ključne tačke. Najbliža nekompletirana tačka je: {finalNextRequiredKeyPoint.Name}.";
        }
        else if (keyPointCompleted && finalNextRequiredKeyPoint != null)
        {
            message = $"Ključna tačka je otključana! " +
                     $"Preostalo je još {tour.KeyPoints.Count - execution.CompletedKeyPoints.Count} ključnih tačaka.";
        }
        else if (keyPointCompleted && finalNextRequiredKeyPoint == null)
        {
            message = "Sve ključne tačke su otključene!";
        }

        // Vrati rezultat
        return new LocationCheckResultDto
        {
            KeyPointCompleted = keyPointCompleted,
            CompletedKeyPointId = keyPointCompleted && execution.CompletedKeyPoints.Any()
                ? execution.CompletedKeyPoints.Last().KeyPointId
                : null,
            LastActivity = execution.LastActivity,
            TotalCompletedKeyPoints = execution.CompletedKeyPoints.Count,
            ProgressPercentage = execution.ProgressPercentage, // za procenat
            NextRequiredKeyPointId = finalNextRequiredKeyPoint?.Id,
            NextRequiredKeyPointName = finalNextRequiredKeyPoint?.Name,
            Message = message
        };
    }


    public bool CanStartTour(long touristId, long tourId)
    {
        // pozivamo AccessService preko repoa
        //var tokens = _tokenRepository.GetByTouristId(touristId);

        //return tokens.Any(t => t.TourId == tourId);

        var tokens = _tokenService.GetTokens(touristId);
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
        if (activeExecution.GroupSessionId != null)
            _groupTourSessionCleanup.HandleComplete(activeExecution.Id);
        activeExecution.Complete();
        var updated = _executionRepository.Update(activeExecution);

        _xpEventService.CreateTourCompletedXp(touristId, tour.Id, 50);

        string message = _achievementService.CompletedTours(touristId);

        if(!String.Equals(message, ""))
            _notificationService.CreateAchievementNotification(touristId, message);

        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionDto AbandonTour(long touristId)
    {
        var activeExecution = _executionRepository.GetActiveByTouristId(touristId);
        if (activeExecution == null)
            throw new NotFoundException("No active tour session found.");

        if (activeExecution.GroupSessionId != null)
            _groupTourSessionCleanup.HandleAbandon(activeExecution.Id);

        activeExecution.Abandon();
        var updated = _executionRepository.Update(activeExecution);
        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionWithNextKeyPointDto? GetActiveWithNextKeyPoint(long touristId)
    {
        try
        {
            var activeExecution = _executionRepository.GetActiveByTouristId(touristId);
            if (activeExecution == null)
                return null;

            var tour = _tourRepository.GetByIdWithKeyPoints(activeExecution.TourId);
            if (tour == null || tour.KeyPoints == null || !tour.KeyPoints.Any())
                return null;

            // Pronađi sledeću nekompletiranu key point (prva po redosledu Id za konzistentnost prikaza)
            var completedKeyPointIds = activeExecution.CompletedKeyPoints?.Select(c => c.KeyPointId).ToList() ?? new List<long>();
            var orderedKeyPoints = tour.KeyPoints.OrderBy(kp => kp.Id).ToList();
            var nextKeyPoint = orderedKeyPoints.FirstOrDefault(kp => !completedKeyPointIds.Contains(kp.Id));

            if (nextKeyPoint == null)
                return null; // Sve tačke su kompletirane

            // Uzmi trenutnu poziciju turiste iz Position servisa (ako postoji)
            double currentLat = activeExecution.StartLatitude;
            double currentLng = activeExecution.StartLongitude;

            // Kreiraj KeyPointDto i sakrij secret ako nije kompletirana
            var keyPointDto = _mapper.Map<KeyPointDto>(nextKeyPoint);
            if (!completedKeyPointIds.Contains(nextKeyPoint.Id))
            {
                keyPointDto.Secret = string.Empty; // Sakrij secret dok nije kompletirana
            }

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
                NextKeyPoint = keyPointDto,
                DistanceToNextKeyPoint = CalculateDistance(
                    currentLat,
                    currentLng,
                    nextKeyPoint.Latitude,
                    nextKeyPoint.Longitude)
            };

            return result;
        }
        catch (Exception)
        {
            // Ako se desi bilo kakva greška, vrati null umesto da baca exception
            return null;
        }
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
