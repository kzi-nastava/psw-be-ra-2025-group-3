
namespace Explorer.Tours.API.Dtos;

public class TourReviewDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public string TouristName { get; set; } // Za prikaz
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public double ProgressPercentage { get; set; }
    public bool IsEdited { get; set; }

    public List<ReviewImageDto> Images { get; set; } = new();
}