using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution execution);
    bool HasActiveSession(long touristId, long tourId);
    TourExecution? GetActiveByTouristId(long touristId);
    TourExecution Update(TourExecution execution); // task2
    TourExecution? GetActiveExecution(long touristId, long tourId); // task2
    TourExecution? GetLatestForTouristAndTour(long touristId, long tourId); //task3
}
