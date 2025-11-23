using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : IPersonRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet;

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public Person Create(Person entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Person? Get(long id)
    {
        var person = _dbSet.Find(id);
        if (person == null) throw new KeyNotFoundException("Not found: " + id);
        return person;
    }

    public Person Update(Person person)
    {
        DbContext.Update(person);
        DbContext.SaveChanges();
        return person;
    }

    public bool EmailExists(string email)
    {
        var normalized = email.Trim().ToLower();
        return _dbSet.Any(p => p.Email.ToLower() == normalized);
    }




    public List<Person> GetAll()
    {
        return _dbSet.ToList();
    }
}