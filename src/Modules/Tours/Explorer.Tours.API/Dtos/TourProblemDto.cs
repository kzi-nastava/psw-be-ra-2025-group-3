namespace Explorer.Tours.API.Dtos;

public class TourProblemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public long AuthorId { get; set; } 
    public int Category { get; set; } // 0 = Transportation, 1 = Accommodation, 2 = Location, 3 = Food, 4 = Other
    public int Priority { get; set; } // 0=Low, 1=Medium, 2=High, 3=Critical
    public string Description { get; set; }
    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Podtask 1
    public int Status { get; set; } // 0=Open, 1=Resolved, 2=Unresolved, 3=Closed
    public string? ResolvedByTouristComment { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
}