namespace Explorer.Tours.API.Dtos;

public class MessageDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public int AuthorType { get; set; } // 0=Tourist, 1=Author, 2=Admin
}