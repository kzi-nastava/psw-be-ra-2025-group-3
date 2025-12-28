using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;

namespace Explorer.Encounters.Core.UseCases;

public class EncounterActivationService : IEncounterActivationService
{
    private readonly IEncounterActivationRepository _activationRepository;
    private readonly IEncounterRepository _encounterRepository;
    private readonly IInternalPositionService _positionService;
    private readonly IMapper _mapper;

    public EncounterActivationService(
        IEncounterActivationRepository activationRepository,
        IEncounterRepository encounterRepository,
        IInternalPositionService positionService,
        IMapper mapper)
    {
        _activationRepository = activationRepository;
        _encounterRepository = encounterRepository;
        _positionService = positionService;
        _mapper = mapper;
    }

    public List<NearbyEncounterDto> GetNearbyEncounters(long touristId, double maxDistanceMeters = 100)
    {
        var touristPosition = _positionService.GetByTouristId(touristId);
        if (touristPosition == null)
            throw new InvalidOperationException("Tourist position not found. Please set your location first.");

        var activeEncounters = _encounterRepository.GetActiveEncounters();

        var nearbyEncounters = new List<NearbyEncounterDto>();

        foreach (var encounter in activeEncounters)
        {
            var distance = DistanceCalculator.CalculateDistance(
                touristPosition.Latitude,
                touristPosition.Longitude,
                encounter.Location.Latitude,
                encounter.Location.Longitude
            );

            bool isCompleted = _activationRepository.HasCompleted(touristId, encounter.Id);

            var dto = new NearbyEncounterDto
            {
                Id = encounter.Id,
                Name = encounter.Name,
                Description = encounter.Description,
                Latitude = encounter.Location.Latitude,
                Longitude = encounter.Location.Longitude,
                XP = encounter.XP,
                Type = encounter.Type.ToString(),
                DistanceInMeters = Math.Round(distance, 2),
                CanActivate = distance <= maxDistanceMeters && !isCompleted,
                IsCompleted = isCompleted
            };

            nearbyEncounters.Add(dto);
        }

        return nearbyEncounters.OrderBy(e => e.DistanceInMeters).ToList();
    }

    public EncounterActivationDto ActivateEncounter(long touristId, long encounterId)
    {
        var encounter = _encounterRepository.GetById(encounterId);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {encounterId} not found.");

        if (encounter.Status != EncounterStatus.Active)
            throw new InvalidOperationException("Only active encounters can be activated.");

        if (_activationRepository.HasCompleted(touristId, encounterId))
            throw new InvalidOperationException("You have already completed this encounter.");

        var existingActivation = _activationRepository.GetActiveByTouristAndEncounter(touristId, encounterId);
        if (existingActivation != null)
            throw new InvalidOperationException("This encounter is already active.");

        var touristPosition = _positionService.GetByTouristId(touristId);
        if (touristPosition == null)
            throw new InvalidOperationException("Tourist position not found. Please set your location first.");

        var distance = DistanceCalculator.CalculateDistance(
            touristPosition.Latitude,
            touristPosition.Longitude,
            encounter.Location.Latitude,
            encounter.Location.Longitude
        );

        if (distance > 100)
            throw new InvalidOperationException($"You are too far from the encounter. Distance: {Math.Round(distance, 2)}m (max: 100m)");

        var activation = new EncounterActivation(encounterId, touristId);
        var result = _activationRepository.Create(activation);

        return _mapper.Map<EncounterActivationDto>(result);
    }

    public List<EncounterActivationDto> GetActiveEncounters(long touristId)
    {
        var activeActivations = _activationRepository.GetActiveByTourist(touristId);
        return _mapper.Map<List<EncounterActivationDto>>(activeActivations);
    }

    public EncounterActivationDto CompleteEncounter(long touristId, long encounterId)
    {
        var activation = _activationRepository.GetActiveByTouristAndEncounter(touristId, encounterId);
        if (activation == null)
            throw new InvalidOperationException("This encounter is not active. You must activate it first.");

        activation.Complete();
        var result = _activationRepository.Update(activation);

        return _mapper.Map<EncounterActivationDto>(result);
    }

    public EncounterActivationDto AbandonEncounter(long touristId, long encounterId)
    {
        var activation = _activationRepository.GetActiveByTouristAndEncounter(touristId, encounterId);
        if (activation == null)
            throw new InvalidOperationException("This encounter is not active.");

        activation.Fail();
        var result = _activationRepository.Update(activation);

        return _mapper.Map<EncounterActivationDto>(result);
    }
}