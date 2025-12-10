using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Execution;

public interface ITourExecutionService
{
    TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId);
    TourExecutionDto? GetActiveTourExecution(long touristId);
    LocationCheckResultDto CheckLocationProgress(LocationCheckDto dto, long touristId); //task2
    TourExecutionDto CompleteTour(long touristId);
    TourExecutionDto AbandonTour(long touristId);
    TourExecutionWithNextKeyPointDto? GetActiveWithNextKeyPoint(long touristId);
}
