namespace Explorer.Tours.API.Dtos;

public class TourProblemCreateDto
{
    public long TourId { get; set; }
    public int Category { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }
}