using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionDbRepository : ITourExecutionRepository
{
    private readonly ToursContext _context;

    public TourExecutionDbRepository(ToursContext context)
    {
        _context = context;
    }

    public TourExecution Create(TourExecution execution)
    {
        _context.TourExecutions.Add(execution);
        _context.SaveChanges();
        return execution;
    }

    public bool HasActiveSession(long touristId, long tourId)
    {
        return _context.TourExecutions
            .Any(te => te.TouristId == touristId
                    && te.TourId == tourId
                    && te.Status == TourExecutionStatus.Active);
    }

    public TourExecution? GetActiveByTouristId(long touristId)
    {
        return _context.TourExecutions
            .Where(te => te.TouristId == touristId && te.Status == TourExecutionStatus.Active)
            .OrderByDescending(te => te.StartTime)
            .FirstOrDefault();
    }

    //task2
    public TourExecution Update(TourExecution execution)
    {
        _context.TourExecutions.Update(execution);
        _context.SaveChanges();
        return execution;
    }
    //task2
    public TourExecution? GetActiveExecution(long touristId, long tourId)
    {
        return _context.TourExecutions
            .FirstOrDefault(te => te.TouristId == touristId
                               && te.TourId == tourId
                               && te.Status == TourExecutionStatus.Active);
    }
    public TourExecution? GetLatestForTouristAndTour(long touristId, long tourId)
    {
        return _context.TourExecutions
            .Where(te => te.TouristId == touristId && te.TourId == tourId)
            .OrderByDescending(te => te.LastActivity)
            .FirstOrDefault();
    }
}