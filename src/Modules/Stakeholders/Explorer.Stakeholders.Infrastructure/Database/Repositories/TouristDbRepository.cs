using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class TouristDbRepository : ITouristRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<Tourist> _dbSet;

    public TouristDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Tourist>();
    }

    public Tourist Get(long personId)
    {
        // Traži turista po PersonId
        return _dbSet.FirstOrDefault(t => t.PersonId == personId);
    }

    public Tourist Update(Tourist tourist)
    {
        _dbContext.Update(tourist);
        _dbContext.SaveChanges();
        return tourist;
    }

    public Tourist Create(Tourist tourist)
    {
        _dbSet.Add(tourist);
        _dbContext.SaveChanges();
        return tourist;
    }
}