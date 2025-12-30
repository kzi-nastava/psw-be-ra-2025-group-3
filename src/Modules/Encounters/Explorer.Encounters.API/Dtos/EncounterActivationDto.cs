namespace Explorer.Encounters.API.Dtos;

public class EncounterActivationDto
{
    public long Id { get; set; }
    public long EncounterId { get; set; }
    public long TouristId { get; set; }
    public string Status { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}