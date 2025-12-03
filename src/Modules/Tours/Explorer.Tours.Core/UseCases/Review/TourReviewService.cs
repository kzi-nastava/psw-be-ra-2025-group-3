// Explorer.Tours.Core/UseCases/Review/TourReviewService.cs
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
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

    public TourReviewService(
        ITourReviewRepository reviewRepository,
        ITourExecutionRepository executionRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _executionRepository = executionRepository;
        _mapper = mapper;
    }

    public TourReviewEligibilityDto CheckEligibility(long tourId, long touristId)
    {
        // Proveri da li postoji TourExecution
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

        // Proveri procenat (>35%)
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

        // Proveri 7 dana od LastActivity
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
        // Proveri eligibility
        var eligibility = CheckEligibility(dto.TourId, touristId);
        if (!eligibility.CanReview)
            throw new InvalidOperationException(eligibility.ReasonIfNot);

        // Proveri da li već postoji recenzija
        if (_reviewRepository.HasReview(touristId, dto.TourId))
            throw new InvalidOperationException("You have already reviewed this tour. Use update instead.");

        // Uzmi najnoviji TourExecution za procenat
        var latestExecution = _executionRepository.GetLatestForTouristAndTour(touristId, dto.TourId);

        // Kreiraj recenziju
        var review = new TourReview(
            dto.TourId,
            touristId,
            dto.Rating,
            dto.Comment,
            latestExecution!.ProgressPercentage
        );

        var created = _reviewRepository.Create(review);
        return _mapper.Map<TourReviewDto>(created);
    }

    public TourReviewDto UpdateReview(TourReviewUpdateDto dto, long touristId)
    {
        // Učitaj postojeću recenziju
        var review = _reviewRepository.GetById(dto.ReviewId);

        if (review == null)
            throw new NotFoundException($"Review with id {dto.ReviewId} not found.");

        if (review.TouristId != touristId)
            throw new InvalidOperationException("You can only update your own reviews.");

        // Proveri eligibility
        var eligibility = CheckEligibility(review.TourId, touristId);
        if (!eligibility.CanReview)
            throw new InvalidOperationException(eligibility.ReasonIfNot);

        // Ažuriraj recenziju
        review.Update(dto.Rating, dto.Comment);
        var updated = _reviewRepository.Update(review);

        return _mapper.Map<TourReviewDto>(updated);
    }

    public List<TourReviewDto> GetReviewsForTour(long tourId)
    {
        var reviews = _reviewRepository.GetAllForTour(tourId);
        return _mapper.Map<List<TourReviewDto>>(reviews);
    }

    public TourReviewDto? GetMyReview(long tourId, long touristId)
    {
        var review = _reviewRepository.GetByTouristAndTour(touristId, tourId);
        return review != null ? _mapper.Map<TourReviewDto>(review) : null;
    }
}