namespace Explorer.Encounters.API.Dtos;

public class NearbyEncounterDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }   // 0 za HiddenLocation
    public double Longitude { get; set; }  // 0 za HiddenLocation
    public int XP { get; set; }
    public string Type { get; set; }
    public double DistanceInMeters { get; set; }
    public bool CanActivate { get; set; }
    public bool IsCompleted { get; set; }

    // Za Social
    public int? RequiredPeopleCount { get; set; }
    public double? RangeInMeters { get; set; }
    public int? CurrentPeopleNearby { get; set; }

    // Za Misc
    public string ActionDescription { get; set; }

    // Za HiddenLocation
    public string ImageUrl { get; set; }
    public string Status { get; set; }  // "TooFar", "Nearby", "Active", "Completed"
}