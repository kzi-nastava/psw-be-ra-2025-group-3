namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces;

public interface IEncounterRepository
{
    Encounter Create(Encounter encounter);
    Encounter Update(Encounter encounter);
    void Delete(long id);
    Encounter? GetById(long id);
    List<Encounter> GetAll();
    List<Encounter> GetActiveEncounters();
}