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
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId)
    {
        var tour = _tourRepository.GetByIdWithKeyPoints(dto.TourId);

        if (tour == null)
            throw new NotFoundException($"Tour with id {dto.TourId} not found.");

        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Cannot start: Tour is not available.");

        var keyPointCount = tour.KeyPoints?.Count ?? 0;
        if (keyPointCount < 2)
            throw new InvalidOperationException("Cannot start: Tour must have at least 2 key points.");

        if (_executionRepository.HasActiveSession(touristId, dto.TourId))
            throw new InvalidOperationException("Cannot start: You already have an active session for this tour.");

        var execution = new TourExecution(
            touristId,
            dto.TourId,
            dto.StartLatitude,
            dto.StartLongitude
        );

        var created = _executionRepository.Create(execution);

        return _mapper.Map<TourExecutionDto>(created);
    }
}
