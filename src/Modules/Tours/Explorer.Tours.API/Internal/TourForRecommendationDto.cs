using System.Collections.Generic;

namespace Explorer.Tours.API.Internal;

public class TourForRecommendationDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public decimal Price { get; set; }
    public double DistanceInKm { get; set; }
    public List<string> Tags { get; set; }
    public List<int> TransportationTypes { get; set; } // 0=Walking, 1=Bicycle, 2=Car, 3=Boat
}