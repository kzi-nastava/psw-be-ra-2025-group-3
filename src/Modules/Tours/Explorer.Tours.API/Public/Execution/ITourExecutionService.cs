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
    LocationCheckResultDto CheckLocationProgress(LocationCheckDto dto, long touristId); //task2
}
