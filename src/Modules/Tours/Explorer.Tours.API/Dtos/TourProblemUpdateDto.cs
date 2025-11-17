namespace Explorer.Tours.API.Dtos;

public class TourProblemUpdateDto
{
    public long Id { get; set; }
    public int Category { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }
}