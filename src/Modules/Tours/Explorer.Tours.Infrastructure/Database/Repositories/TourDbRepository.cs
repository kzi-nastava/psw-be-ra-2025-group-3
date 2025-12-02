using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore; // OBAVEZNO ZA .Include()

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    private readonly ToursContext _context;

    public TourDbRepository(ToursContext context)
    {
        _context = context;
    }

    public Tour Create(Tour tour)
    {
        _context.Tours.Add(tour);
        _context.SaveChanges();
        return tour;
    }

    public Tour Update(Tour tour)
    {
        try
        {
            _context.Tours.Update(tour);
            _context.SaveChanges();
            return tour;
        }
        catch (DbUpdateException e)
        {
            throw new KeyNotFoundException(e.Message);
        }
    }

    public void Delete(long id)
    {
        var tour = _context.Tours.Find(id);
        if (tour != null)
        {
            _context.Tours.Remove(tour);
            _context.SaveChanges();
        }
    }

    // === IZMENJENO: Dodat .Include(t => t.Equipment) ===
    public Tour? GetById(long id)
    {
        return _context.Tours
            .Include(t => t.Equipment) // <--- KLJUČNO ZA EDIT
            .FirstOrDefault(t => t.Id == id);
    }

    public List<Tour> GetByAuthorId(long authorId)
    {
        return _context.Tours
            .Include(t => t.Equipment) 
            .Where(t => t.AuthorId == authorId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public IEnumerable<Tour> GetPublished()
    {
        return _context.Tours
                  .Where(t => t.Status == TourStatus.Published)
                  .ToList();
    }

    public Tour? GetByIdWithKeyPoints(long id)
    {
        return _context.Tours
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .FirstOrDefault(t => t.Id == id);  //za tour-execution
    }
}