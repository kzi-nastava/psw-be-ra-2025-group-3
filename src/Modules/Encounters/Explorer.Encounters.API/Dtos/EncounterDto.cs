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
}