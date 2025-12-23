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

    public Tour? GetById(long id)
    {
        return _context.Tours.Find(id);
    }
    public Tour? GetWithEquipment(long id)
    {
        return _context.Tours
            .Include(t => t.Equipment)
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

    public List<Tour> GetPublishedWithKeyPoints()
    {
        return _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
    }

    public Tour? GetTourWithKeyPoints(long id)
    {
        return _context.Tours
            .Include(t => t.KeyPoints)
            .Include(t => t.Equipment)
            .FirstOrDefault(t => t.Id == id);
    }
    public List<Tour> SearchAndFilter(string? name, List<string>? tags, int? minDifficulty, 
                                       int? maxDifficulty, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Tours
            .Where(t => t.Status == TourStatus.Published);

        // Filtriranje po nazivu (case-insensitive)
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.ToLower().Contains(name.ToLower()));
        }

        // Filtriranje po težini (minimum)
        if (minDifficulty.HasValue)
        {
            query = query.Where(t => (int)t.Difficulty >= minDifficulty.Value);
        }

        // Filtriranje po težini (maximum)
        if (maxDifficulty.HasValue)
        {
            query = query.Where(t => (int)t.Difficulty <= maxDifficulty.Value);
        }

        // Filtriranje po ceni (minimum)
        if (minPrice.HasValue)
        {
            query = query.Where(t => t.Price >= minPrice.Value);
        }

        // Filtriranje po ceni (maximum)
        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= maxPrice.Value);
        }

        // Izvlačimo iz baze i filtriramo tagove u memoriji
        var tours = query.ToList();

        // Filtriranje po tagovima (u memoriji jer EF Core ne može da parsira JSON)
        if (tags != null && tags.Any())
        {
            tours = tours.Where(t => t.Tags.Any(tag => tags.Contains(tag))).ToList();
        }

        return tours;
    }
}