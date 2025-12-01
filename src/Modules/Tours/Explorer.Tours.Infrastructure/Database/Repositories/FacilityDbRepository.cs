using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories;


public class FacilityDbRepository : IFacilityRepository
{
    private readonly ToursContext _dbContext;

    public FacilityDbRepository(ToursContext dbContext)
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
        return _dbContext.Facilities
            .AsNoTracking()
            .ToList();
    }

    public Facility Get(long id)
    {
        return _dbContext.Facilities
            .AsNoTracking()
            .FirstOrDefault(f => f.Id == id);
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
        var existing = _dbContext.Facilities.FirstOrDefault(f => f.Id == facility.Id);
        if (existing == null) return null;

        existing.Name = facility.Name;
        existing.Latitude = facility.Latitude;
        existing.Longitude = facility.Longitude;
        existing.Category = facility.Category;

        _dbContext.SaveChanges();
        return existing;
    }
}