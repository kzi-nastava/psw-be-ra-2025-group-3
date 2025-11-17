using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TouristEquipmentDbRepository : ITouristEquipmentRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TouristEquipment> _dbSet;

    public TouristEquipmentDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TouristEquipment>();
    }

    public List<TouristEquipment> GetByTouristId(long touristId)
    {
        return _dbSet
            .Include(te => te.Equipment)
            .Where(te => te.TouristId == touristId)
            .ToList();
    }

    public TouristEquipment GetByTouristAndEquipment(long touristId, long equipmentId)
    {
        return _dbSet
            .Include(te => te.Equipment)
            .FirstOrDefault(te => te.TouristId == touristId && te.EquipmentId == equipmentId);
    }

    public TouristEquipment Create(TouristEquipment touristEquipment)
    {
        _dbSet.Add(touristEquipment);
        _dbContext.SaveChanges();
        return touristEquipment;
    }

    public void Delete(long touristId, long equipmentId)
    {
        var entity = _dbSet.FirstOrDefault(te =>
            te.TouristId == touristId && te.EquipmentId == equipmentId);

        if (entity != null)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}