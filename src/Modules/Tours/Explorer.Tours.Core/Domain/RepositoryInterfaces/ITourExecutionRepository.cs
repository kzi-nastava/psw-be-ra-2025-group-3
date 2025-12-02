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
}
