using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using System.Diagnostics;

namespace Explorer.Encounters.Core.UseCases;

public class EncounterActivationService : IEncounterActivationService
{
    private readonly IEncounterActivationRepository _activationRepository;
    private readonly IEncounterRepository _encounterRepository;
    private readonly IInternalPositionService _positionService;
    private readonly IInternalTouristXPAndLevelSerive _touristXPService;
    private readonly IMapper _mapper;

    public EncounterActivationService(
        IEncounterActivationRepository activationRepository,
        IEncounterRepository encounterRepository,
        IInternalPositionService positionService,
        IInternalTouristXPAndLevelSerive touristXPService,
        IMapper mapper)
    {
        _activationRepository = activationRepository;
        _encounterRepository = encounterRepository;
        _positionService = positionService;
        _touristXPService = touristXPService;
        _mapper = mapper;
    }

    public EncounterActivationDto ActivateEncounter(long touristId, long encounterId)
    {
        // Validacije
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

        // Provera distance
        var distance = DistanceCalculator.CalculateDistance(
            touristPosition.Latitude,
            touristPosition.Longitude,
            encounter.Location.Latitude,
            encounter.Location.Longitude
        );

        // Različite distance za različite tipove
        double maxActivationDistance;

        if (encounter.Type == EncounterType.HiddenLocation)
        {
            // Hidden Location: Mora biti u radijusu od 100m da bi ga video i aktivirao
            maxActivationDistance = 100;
        }
        else if (encounter.Type == EncounterType.Social && encounter.RangeInMeters.HasValue)
        {
            // Social: Može aktivirati iz daljine
            maxActivationDistance = Math.Max(encounter.RangeInMeters.Value, 100);
        }
        else
        {
            // Misc: Default 100m
            maxActivationDistance = 100;
        }

        if (distance > maxActivationDistance)
            throw new InvalidOperationException(
                $"You are too far from the encounter. Distance: {Math.Round(distance, 2)}m (max: {maxActivationDistance}m)");

        // Kreiraj aktivaciju
        var activation = new EncounterActivation(encounterId, touristId);
        activation.UpdateLocation(touristPosition.Latitude, touristPosition.Longitude);

        // ===== SOCIAL ENCOUNTER: Auto-complete ako ima dovoljno ljudi =====
        if (encounter.Type == EncounterType.Social)
        {
            int peopleNearby = CountPeopleNearEncounter(encounter.Id, touristId
            );

            if (peopleNearby >= encounter.RequiredPeopleCount)
            {
                activation.Complete();
                _touristXPService.AddExperience(touristId, encounter.XP);
            }
        }

        var result = _activationRepository.Create(activation);
        return _mapper.Map<EncounterActivationDto>(result);
    }

    public EncounterActivationDto CompleteEncounter(long touristId, long encounterId)
    {
        var activation = _activationRepository.GetActiveByTouristAndEncounter(touristId, encounterId);
        if (activation == null)
            throw new InvalidOperationException("This encounter is not active. You must activate it first.");

        var encounter = _encounterRepository.GetById(encounterId);

        activation.Complete();
        var result = _activationRepository.Update(activation);

        _touristXPService.AddExperience(touristId, encounter.XP);

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

    public List<EncounterActivationDto> GetActiveEncounters(long touristId)
    {
        var activations = _activationRepository.GetActiveByTourist(touristId);
        return _mapper.Map<List<EncounterActivationDto>>(activations);
    }

    public List<NearbyEncounterDto> GetNearbyEncounters(long touristId, double maxDistanceMeters = 100)
    {
        var touristPosition = _positionService.GetByTouristId(touristId);
        if (touristPosition == null)
            throw new InvalidOperationException("Tourist position not found. Please set your location first.");

        // ===== AUTOMATSKI PROVERI I COMPLETE-UJ HIDDEN LOCATION ENCOUNTERS =====
        CheckAndCompleteHiddenLocationEncounters(touristId, touristPosition);

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

            // ===== MISC i SOCIAL: Prikaži SVE, ALI canActivate SAMO ako je blizu =====
            if (encounter.Type == EncounterType.Misc || encounter.Type == EncounterType.Social)
            {
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
                    CanActivate = !isCompleted && distance <= maxDistanceMeters,
                    IsCompleted = isCompleted,
                    RequiredPeopleCount = encounter.RequiredPeopleCount,
                    RangeInMeters = encounter.RangeInMeters,
                    ImageUrl = encounter.ImageUrl,
                    ActionDescription = encounter.ActionDescription,
                    Status = null,
                    CurrentPeopleNearby = null
                };

                // Za Social: Dodaj trenutni broj ljudi u blizini
                if (encounter.Type == EncounterType.Social)
                {
                    dto.CurrentPeopleNearby = CountPeopleNearEncounter(encounter.Id, touristId);
                }

                nearbyEncounters.Add(dto);
            }
            // ===== HIDDEN LOCATION: Prikaži SAMO ako je u radijusu od 100m =====
            else if (encounter.Type == EncounterType.HiddenLocation && distance <= 100)
            {
                var dto = new NearbyEncounterDto
                {
                    Id = encounter.Id,
                    Name = encounter.Name,
                    Description = encounter.Description,
                    Latitude = 0,
                    Longitude = 0,
                    XP = encounter.XP,
                    Type = encounter.Type.ToString(),
                    DistanceInMeters = Math.Round(distance, 2),
                    CanActivate = !isCompleted && distance <= 100,
                    IsCompleted = isCompleted,
                    ImageUrl = encounter.ImageUrl,
                    Status = DetermineHiddenLocationStatus(touristId, encounter.Id, distance, isCompleted),
                    RequiredPeopleCount = null,
                    RangeInMeters = null,
                    ActionDescription = null,
                    CurrentPeopleNearby = null
                };

                nearbyEncounters.Add(dto);
            }
        }

        return nearbyEncounters.OrderBy(e => e.DistanceInMeters).ToList();
    }

    // ===== HELPER METODE =====

    private void CheckAndCompleteHiddenLocationEncounters(long touristId, PositionDto touristPosition)
    {
        var activeActivations = _activationRepository.GetActiveByTourist(touristId);

        foreach (var activation in activeActivations)
        {
            var encounter = _encounterRepository.GetById(activation.EncounterId);

            if (encounter == null || encounter.Type != EncounterType.HiddenLocation)
                continue;

            var distance = DistanceCalculator.CalculateDistance(
                touristPosition.Latitude,
                touristPosition.Longitude,
                encounter.Location.Latitude,
                encounter.Location.Longitude
            );

            const double MAX_DISTANCE_METERS = 5.0;
            const int REQUIRED_SECONDS = 30;

            if (distance <= MAX_DISTANCE_METERS)
            {
                // Ažuriraj trenutnu poziciju
                activation.UpdateLocation(touristPosition.Latitude, touristPosition.Longitude);

                // ===== DODAJ OVO: Markaj prvi put kada je stigao =====
                activation.MarkFirstTimeAtCorrectLocation();

                _activationRepository.Update(activation);

                // Proveri da li je 30 sekundi na pravom mestu
                if (activation.IsAtCorrectLocationForDuration(
                    encounter.Location.Latitude,
                    encounter.Location.Longitude,
                    MAX_DISTANCE_METERS,
                    REQUIRED_SECONDS))
                {
                    activation.Complete();
                    _activationRepository.Update(activation);
                    _touristXPService.AddExperience(touristId, encounter.XP);
                }
            }
            else
            {
                // ===== DODAJ OVO: Ako je daleko, resetuj timer =====
                if (activation.FirstTimeAtCorrectLocationAt.HasValue)
                {
                    activation.ResetCorrectLocationTimer();
                    _activationRepository.Update(activation);
                }
            }
        }
    }

    public int CountPeopleNearEncounter(long encounterId, long touristId)
    {
        var encounter = _encounterRepository.GetById(encounterId);
        var allPositions = _positionService.GetAll();

        int count = 0;
        foreach (var position in allPositions)
        {
            if (position.TouristId == touristId)
            {
                continue;
            }

            double distance = DistanceCalculator.CalculateDistance(
                encounter.Location.Latitude,
                encounter.Location.Longitude,
                position.Latitude,
                position.Longitude
            );

            if (distance <= encounter.RangeInMeters)
            {
                count++;
            }
        }

        return count;
    }

    private string DetermineHiddenLocationStatus(long touristId, long encounterId, double distance, bool isCompleted)
    {
        if (isCompleted)
            return "Completed";

        var activation = _activationRepository.GetActiveByTouristAndEncounter(touristId, encounterId);

        if (activation != null && activation.Status == EncounterActivationStatus.InProgress)
            return "Active";

        if (distance <= 100)
            return "Nearby";

        return "TooFar";
    }
}