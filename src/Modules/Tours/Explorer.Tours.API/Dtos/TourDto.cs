namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; } // 0 = Easy, 1 = Medium, 2 = Hard
    public int Status { get; set; } // 0 = Draft, 1 = Published
    public decimal Price { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Tags { get; set; }
}