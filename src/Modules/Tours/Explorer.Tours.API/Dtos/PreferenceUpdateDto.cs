using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class PreferenceUpdateDto
{
    public long Id { get; set; }
    public int Difficulty { get; set; } // 0=Easy, 1=Medium, 2=Hard
    public int WalkingRating { get; set; } // 0-3
    public int BicycleRating { get; set; } // 0-3
    public int CarRating { get; set; } // 0-3
    public int BoatRating { get; set; } // 0-3
    public List<string> Tags { get; set; }
}