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
        // može i FirstOrDefault + bacanje izuzetka, ali ovo je dovoljno za sad
        return _dbContext.Facilities.Find(id)!;
    }

    public Facility Update(Facility facility)
    {
        _dbContext.Facilities.Update(facility);
        _dbContext.SaveChanges();
        return facility;
    }

    public void Delete(long id)
    {
        var entity = _dbContext.Facilities.Find(id);
        if (entity != null)
        {
            _dbContext.Facilities.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
