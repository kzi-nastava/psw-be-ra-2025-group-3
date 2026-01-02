using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public;

public interface IEncounterActivationService
{
    List<NearbyEncounterDto> GetNearbyEncounters(long touristId, double maxDistanceMeters = 100);
    EncounterActivationDto ActivateEncounter(long touristId, long encounterId);
    List<EncounterActivationDto> GetActiveEncounters(long touristId);
    EncounterActivationDto CompleteEncounter(long touristId, long encounterId);
    EncounterActivationDto AbandonEncounter(long touristId, long encounterId);
}