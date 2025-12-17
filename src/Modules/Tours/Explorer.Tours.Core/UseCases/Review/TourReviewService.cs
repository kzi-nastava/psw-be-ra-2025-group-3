using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Review;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Review;

public class TourReviewService : ITourReviewService
{
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly IMapper _mapper;
    private readonly ITourRepository _tourRepository;

    public TourReviewService(
        ITourReviewRepository reviewRepository,
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public TourReviewEligibilityDto CheckEligibility(long tourId, long touristId)
    {
        var latestExecution = _executionRepository.GetLatestForTouristAndTour(touristId, tourId);

        if (latestExecution == null)
        {
            return new TourReviewEligibilityDto
            {
                CanReview = false,
                ReasonIfNot = "You must purchase and start the tour before leaving a review.",
                CurrentProgress = 0,
                DaysSinceLastActivity = 0
            };
        }

        if (latestExecution.ProgressPercentage <= 35)
        {
            return new TourReviewEligibilityDto
            {
                CanReview = false,
                ReasonIfNot = $"You must complete more than 35% of the tour. Current progress: {latestExecution.ProgressPercentage:F1}%.",
                CurrentProgress = latestExecution.ProgressPercentage,
                DaysSinceLastActivity = (int)(DateTime.UtcNow - latestExecution.LastActivity).TotalDays
            };
        }

        var daysSinceLastActivity = (DateTime.UtcNow - latestExecution.LastActivity).TotalDays;
        if (daysSinceLastActivity > 7)
        {
            return new TourReviewEligibilityDto
            {
                CanReview = false,
                ReasonIfNot = $"You can only review within 7 days of your last activity. Last activity was {(int)daysSinceLastActivity} days ago.",
                CurrentProgress = latestExecution.ProgressPercentage,
                DaysSinceLastActivity = (int)daysSinceLastActivity
            };
        }

        return new TourReviewEligibilityDto
        {
            CanReview = true,
            ReasonIfNot = null,
            CurrentProgress = latestExecution.ProgressPercentage,
            DaysSinceLastActivity = (int)daysSinceLastActivity
        };
    }

    public TourReviewDto CreateReview(TourReviewCreateDto dto, long touristId)
    {
        var eligibility = CheckEligibility(dto.TourId, touristId);
        if (!eligibility.CanReview)
            throw new InvalidOperationException(eligibility.ReasonIfNot);

        if (_reviewRepository.HasReview(touristId, dto.TourId))
            throw new InvalidOperationException("You have already reviewed this tour. Use update instead.");

        var latestExecution = _executionRepository.GetLatestForTouristAndTour(touristId, dto.TourId);

        var review = new TourReview(
            dto.TourId,
            touristId,
            dto.Rating,
            dto.Comment,
            latestExecution!.ProgressPercentage
        );

        var created = _reviewRepository.Create(review);
        return MapReviewToDto(created);
    }

    public TourReviewDto UpdateReview(TourReviewUpdateDto dto, long touristId)
    {
        var review = _reviewRepository.GetById(dto.ReviewId);

        if (review == null)
            throw new NotFoundException($"Review with id {dto.ReviewId} not found.");

        if (review.TouristId != touristId)
            throw new InvalidOperationException("You can only update your own reviews.");

        var eligibility = CheckEligibility(review.TourId, touristId);
        if (!eligibility.CanReview)
            throw new InvalidOperationException(eligibility.ReasonIfNot);

        review.Update(dto.Rating, dto.Comment);
        var updated = _reviewRepository.Update(review);

        return MapReviewToDto(updated);
    }

    public List<TourReviewDto> GetReviewsForTour(long tourId)
    {
        var reviews = _reviewRepository.GetAllForTour(tourId);
        return reviews.Select(r => MapReviewToDto(r)).ToList();
    }

    public TourReviewDto? GetMyReview(long tourId, long touristId)
    {
        var review = _reviewRepository.GetByTouristAndTour(touristId, tourId);
        return review != null ? MapReviewToDto(review) : null;
    }

    public List<TourReviewDto> GetAllReviewsForTourist(long touristId)
    {
        var reviews = _reviewRepository.GetAllForTourist(touristId);
        return reviews.Select(r => MapReviewToDto(r)).ToList();
    }

    public ReviewImageDto AddImageToReview(long reviewId, long touristId, string imageUrl)
    {
        var review = _reviewRepository.GetByIdWithImages(reviewId);

        if (review == null)
            throw new NotFoundException($"Review with id {reviewId} not found.");

        if (review.TouristId != touristId)
            throw new InvalidOperationException("You can only add images to your own reviews.");

        var eligibility = CheckEligibility(review.TourId, touristId);
        if (!eligibility.CanReview)
            throw new InvalidOperationException(eligibility.ReasonIfNot);

        review.AddImage(imageUrl);
        var updated = _reviewRepository.Update(review);

        var addedImage = updated.Images.Last();
        return _mapper.Map<ReviewImageDto>(addedImage);
    }

    public void DeleteImageFromReview(long reviewId, long imageId, long touristId)
    {
        var review = _reviewRepository.GetByIdWithImages(reviewId);

        if (review == null)
            throw new NotFoundException($"Review with id {reviewId} not found.");

        if (review.TouristId != touristId)
            throw new InvalidOperationException("You can only delete images from your own reviews.");

        var image = review.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new NotFoundException($"Image with id {imageId} not found.");

        review.RemoveImage(imageId);
        _reviewRepository.Update(review);
    }

    public ReviewImageDto? GetImageById(long reviewId, long imageId)
    {
        var review = _reviewRepository.GetByIdWithImages(reviewId);
        var image = review?.Images.FirstOrDefault(i => i.Id == imageId);
        return image != null ? _mapper.Map<ReviewImageDto>(image) : null;
    }

    private TourReviewDto MapReviewToDto(TourReview review)
    {
        var dto = _mapper.Map<TourReviewDto>(review);
        var tour = _tourRepository.GetById(review.TourId); 
        dto.TourName = tour?.Name ?? $"Tour #{review.TourId}";
        var latestExecution = _executionRepository.GetLatestForTouristAndTour(review.TouristId, review.TourId);
        if (latestExecution != null)
        {
            dto.ProgressPercentage = latestExecution.ProgressPercentage;
        }

        return dto;
    }
}