using System.Collections.Generic;

namespace Explorer.Tours.API.Internal;

public interface IInternalTourService
{
    List<TourForRecommendationDto> GetPublishedToursForRecommendation();
}