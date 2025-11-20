using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Infrastructure.Database.Repositories;


public class FacilityDbRepository : IFacilityRepository
{
    private readonly BlogContext _dbContext;

    public FacilityDbRepository(BlogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Facility Create(Facility facility)
    {
        _dbContext.Facilities.Add(facility);
        _dbContext.SaveChanges();
        return facility;
    }

    public List<Facility> GetAll()
    {
        return _dbContext.Facilities.ToList();
    }

    public Facility Get(long id)
    {
        return _dbContext.Facilities.FirstOrDefault(f => f.Id == id);
    }

    public void Delete(long id)
    {
        var entity = _dbContext.Facilities.FirstOrDefault(f => f.Id == id);

        if (entity == null)
            throw new KeyNotFoundException("Facility not found.");

        _dbContext.Facilities.Remove(entity);
        _dbContext.SaveChanges();
    }


    public Facility Update(Facility facility)
    {
        _dbContext.Facilities.Update(facility);
        _dbContext.SaveChanges();
        return facility;
    }

    
}
