using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories;

public class EncounterRepository : IEncounterRepository
{
    private readonly EncountersContext _dbContext;

    public EncounterRepository(EncountersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Encounter Create(Encounter encounter)
    {
        _dbContext.Encounters.Add(encounter);
        _dbContext.SaveChanges();
        return encounter;
    }

    public Encounter Update(Encounter encounter)
    {
        _dbContext.Encounters.Update(encounter);
        _dbContext.SaveChanges();
        return encounter;
    }

    public void Delete(long id)
    {
        var encounter = GetById(id);
        if (encounter != null)
        {
            _dbContext.Encounters.Remove(encounter);
            _dbContext.SaveChanges();
        }
    }

    public Encounter? GetById(long id)
    {
        return _dbContext.Encounters.Find(id);
    }

    public List<Encounter> GetAll()
    {
        return _dbContext.Encounters.ToList();
    }

    public List<Encounter> GetActiveEncounters()
    {
        return _dbContext.Encounters
            .Where(e => e.Status == EncounterStatus.Active)
            .ToList();
    }
}