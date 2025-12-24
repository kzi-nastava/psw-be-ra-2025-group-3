using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Dtos;

public class RecommendedTourDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public decimal Price { get; set; }
    public double DistanceInKm { get; set; }
    public List<string> Tags { get; set; }
    public int MatchScore { get; set; }
}