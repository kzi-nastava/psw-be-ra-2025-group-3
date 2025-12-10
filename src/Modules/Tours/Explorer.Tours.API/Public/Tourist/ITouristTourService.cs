using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristTourService
{
    List<TourPreviewDto> GetPublishedTours();
    List<TourPreviewDto> GetAvailableTours();
    TourPreviewDto GetPreview(long tourId);
    TourDetailsDto GetDetails(long touristId, long tourId);
}