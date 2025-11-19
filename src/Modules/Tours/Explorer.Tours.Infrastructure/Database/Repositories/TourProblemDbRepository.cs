using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourProblemDbRepository : ITourProblemRepository
{
    private readonly ToursContext _context;

    public TourProblemDbRepository(ToursContext context)
    {
        _context = context;
    }

    public TourProblem Create(TourProblem problem)
    {
        _context.TourProblems.Add(problem);
        _context.SaveChanges();
        return problem;
    }

    public TourProblem Update(TourProblem problem)
    {
        _context.TourProblems.Update(problem);
        _context.SaveChanges();
        return problem;
    }

    public void Delete(long id)
    {
        var problem = _context.TourProblems.Find(id);
        if (problem != null)
        {
            _context.TourProblems.Remove(problem);
            _context.SaveChanges();
        }
    }

    public TourProblem? GetById(long id)
    {
        return _context.TourProblems.Find(id);
    }

    public List<TourProblem> GetByTouristId(long touristId)
    {
        return _context.TourProblems
            .Where(p => p.TouristId == touristId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }
}