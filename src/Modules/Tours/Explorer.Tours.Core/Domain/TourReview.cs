using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourReview : Entity
{
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public int Rating { get; private set; } // 1-5
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public double ProgressPercentage { get; private set; } // % pređene ture
    public bool IsEdited { get; private set; }

    public List<ReviewImage> Images { get; private set; } = new();

    private TourReview() { }

    public TourReview(long tourId, long touristId, int rating, string? comment, double progressPercentage)
    {
        if (tourId == 0)
            throw new ArgumentException("Tour ID must be valid.", nameof(tourId));
        if (touristId == 0)
            throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        if (progressPercentage < 0 || progressPercentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100.", nameof(progressPercentage));

        TourId = tourId;
        TouristId = touristId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        ProgressPercentage = progressPercentage;
        IsEdited = false;
        Images = new List<ReviewImage>();
    }

    public void Update(int rating, string? comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        Rating = rating;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
        IsEdited = true;
    }
    public void AddImage(string imageUrl)
    {
        if (Images.Count >= 5)
            throw new InvalidOperationException("Maximum 5 images allowed per review.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty.", nameof(imageUrl));

        var image = new ReviewImage(Id, imageUrl);
        Images.Add(image);
    }

    public void RemoveImage(long imageId)
    {
        var image = Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new ArgumentException($"Image with ID {imageId} not found.");

        Images.Remove(image);
    }
}