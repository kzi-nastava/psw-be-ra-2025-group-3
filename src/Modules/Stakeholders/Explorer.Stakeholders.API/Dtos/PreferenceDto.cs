using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Dtos;

public class PreferenceDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public int Difficulty { get; set; }
    public int WalkingRating { get; set; }
    public int BicycleRating { get; set; }
    public int CarRating { get; set; }
    public int BoatRating { get; set; }
    public List<string> Tags { get; set; }
}