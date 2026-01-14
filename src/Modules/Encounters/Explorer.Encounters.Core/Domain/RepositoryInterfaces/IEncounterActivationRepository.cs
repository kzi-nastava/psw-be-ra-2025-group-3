namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces;

public interface IEncounterActivationRepository
{
    EncounterActivation Create(EncounterActivation activation);
    EncounterActivation Update(EncounterActivation activation);
    EncounterActivation? GetById(long id);
    EncounterActivation? GetActiveByTouristAndEncounter(long touristId, long encounterId);
    List<EncounterActivation> GetActiveByTourist(long touristId);
    List<EncounterActivation> GetCompletedByTourist(long touristId);
    bool HasCompleted(long touristId, long encounterId);

    // Nove metode za Social Encounter
    List<EncounterActivation> GetActiveActivationsForEncounter(long encounterId);
    List<long> GetActiveTouristIdsInRange(long encounterId, double centerLat, double centerLon, double rangeInMeters);
}