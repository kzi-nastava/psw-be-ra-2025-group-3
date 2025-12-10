namespace Explorer.Tours.API.Dtos;

public class ReviewImageDto
{
    public long Id { get; set; }
    public long TourReviewId { get; set; }
    public string ImageUrl { get; set; }
    public DateTime UploadedAt { get; set; }
}