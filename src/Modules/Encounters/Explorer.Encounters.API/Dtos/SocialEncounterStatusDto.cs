namespace Explorer.Encounters.API.Dtos;

public class SocialEncounterStatusDto
{
    public long EncounterId { get; set; }
    public int ActiveTouristsInRange { get; set; }
    public int RequiredPeopleCount { get; set; }
    public bool IsCompleted { get; set; }
    public List<long> TouristIdsInRange { get; set; } = new();
}