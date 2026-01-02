namespace Explorer.Encounters.API.Dtos;

public class NearbyEncounterDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int XP { get; set; }
    public string Type { get; set; }
    public double DistanceInMeters { get; set; }
    public bool CanActivate { get; set; } // Da li je dovoljno blizu
    public bool IsCompleted { get; set; } // Da li je vec resio
}