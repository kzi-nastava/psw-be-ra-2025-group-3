using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class TourPreviewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public List<string> Tags { get; set; }
    public string Difficulty { get; set; }
    public double AverageRating { get; set; }
    public KeyPointDto FirstKeyPoint { get; set; }
    public List<TourReviewDto> Reviews { get; set; }

    public double Length { get; set; }
    public double AverageDuration { get; set; }
    public string StartPoint { get; set; }
    public List<string> Images { get; set; }
}