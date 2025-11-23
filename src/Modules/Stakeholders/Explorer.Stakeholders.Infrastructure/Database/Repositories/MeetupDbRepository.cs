using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class MeetupDbRepository : IMeetupRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<Meetup> _dbSet;

    public MeetupDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Meetup>();
    }

    public List<Meetup> GetAll()
    {
        return _dbSet.AsNoTracking().ToList();
    }

    public Meetup? GetById(long id)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(m => m.Id == id);
    }

    public Meetup Create(Meetup meetup)
    {
        _dbSet.Add(meetup);
        _dbContext.SaveChanges();
        return meetup;
    }

    public Meetup Update(Meetup meetup)
    {
        _dbContext.Update(meetup);
        _dbContext.SaveChanges();
        return meetup;
    }

    public void Delete(Meetup meetup)
    {
        _dbSet.Remove(meetup);
        _dbContext.SaveChanges();
    }
}
