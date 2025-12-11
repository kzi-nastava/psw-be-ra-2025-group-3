namespace Explorer.Tours.API.Dtos;

public class NotificationDto
{
    public long Id { get; set; }
    public long RecipientId { get; set; }
    public int Type { get; set; }
    public long RelatedEntityId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}