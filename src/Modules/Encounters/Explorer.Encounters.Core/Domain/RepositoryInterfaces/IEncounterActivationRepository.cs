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
}