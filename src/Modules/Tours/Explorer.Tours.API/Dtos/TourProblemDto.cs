namespace Explorer.Tours.API.Dtos;

public class TourProblemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public int Category { get; set; } // 0=Transportation, 1=Accommodation, 2=Guide, 3=Location, 4=Food, 5=Other
    public int Priority { get; set; } // 0=Low, 1=Medium, 2=High, 3=Critical
    public string Description { get; set; }
    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}