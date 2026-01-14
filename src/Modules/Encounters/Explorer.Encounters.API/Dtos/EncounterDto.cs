namespace Explorer.Encounters.API.Dtos;

public class EncounterDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int XP { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }

    // Za Misc encounter
    public string? ActionDescription { get; set; }

    // Za Social encounter
    public int? RequiredPeopleCount { get; set; }
    public double? RangeInMeters { get; set; }

    // Za HiddenLocation encounter
    public string? ImageUrl { get; set; }
}