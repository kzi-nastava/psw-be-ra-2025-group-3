// Explorer.Tours.Core/Domain/ReviewImage.cs
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ReviewImage : Entity
{
    public long TourReviewId { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime UploadedAt { get; private set; }

    // Navigation property
    public TourReview TourReview { get; private set; }

    // EF Core konstruktor
    private ReviewImage() { }

    public ReviewImage(long tourReviewId, string imageUrl)
    {
        if (tourReviewId == 0)
            throw new ArgumentException("TourReview ID must be valid.", nameof(tourReviewId));
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty.", nameof(imageUrl));

        TourReviewId = tourReviewId;
        ImageUrl = imageUrl;
        UploadedAt = DateTime.UtcNow;
    }
}