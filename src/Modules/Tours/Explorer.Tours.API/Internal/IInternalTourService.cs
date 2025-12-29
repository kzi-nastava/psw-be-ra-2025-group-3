using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Internal;

public interface IInternalTourService
{
    List<TourForRecommendationDto> GetPublishedToursForRecommendation();
    TourDto GetById(long tourId);
}