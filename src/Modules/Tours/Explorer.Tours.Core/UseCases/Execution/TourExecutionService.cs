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

namespace Explorer.Tours.Core.UseCases.Execution;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        ITourPurchaseTokenRepository tokenRepository,
        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _tokenRepository = tokenRepository;
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
        //  Učitaj aktivnu TourExecution sesiju
        var execution = _executionRepository.GetActiveExecution(touristId, dto.TourId); // tour id u dto

        if (execution == null)
            throw new InvalidOperationException("No active tour session found.");

        //  Učitaj KeyPoints ture
        var tour = _tourRepository.GetByIdWithKeyPoints(execution.TourId);

        if (tour == null || tour.KeyPoints == null)
            throw new InvalidOperationException("Tour or KeyPoints not found.");

        // 3. Pozovi agregat metodu
        bool keyPointCompleted = execution.CheckLocationProgress(
            dto.CurrentLatitude,
            dto.CurrentLongitude,
            tour.KeyPoints.ToList()
        );

        //  Sačuvaj izmene
        _executionRepository.Update(execution);

        //  Vrati rezultat
        return new LocationCheckResultDto
        {
            KeyPointCompleted = keyPointCompleted,
            CompletedKeyPointId = keyPointCompleted
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
}
