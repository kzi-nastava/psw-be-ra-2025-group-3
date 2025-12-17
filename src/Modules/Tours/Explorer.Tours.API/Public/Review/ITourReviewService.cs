// Explorer.Tours.API/Public/Review/ITourReviewService.cs
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Review;

public interface ITourReviewService
{
    TourReviewEligibilityDto CheckEligibility(long tourId, long touristId);
    TourReviewDto CreateReview(TourReviewCreateDto dto, long touristId);
    TourReviewDto UpdateReview(TourReviewUpdateDto dto, long touristId);
    List<TourReviewDto> GetReviewsForTour(long tourId);
    TourReviewDto? GetMyReview(long tourId, long touristId);
    ReviewImageDto AddImageToReview(long reviewId, long touristId, string imageUrl);
    void DeleteImageFromReview(long reviewId, long imageId, long touristId);
    ReviewImageDto? GetImageById(long reviewId, long imageId);
    List<TourReviewDto> GetAllReviewsForTourist(long touristId);
}