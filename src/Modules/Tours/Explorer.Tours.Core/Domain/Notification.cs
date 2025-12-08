using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Notification : Entity
{
    public long RecipientId { get; private set; }
    public NotificationType Type { get; private set; }
    public long RelatedEntityId { get; private set; } //TourProblemID za nas task (Tour Issue Lifecycle)
    public string Message { get; private set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; set; }

    public Notification(long recipientId, NotificationType type, long relatedEntityId, string message)
    {
        RecipientId = recipientId;
        Type = type;
        RelatedEntityId = relatedEntityId;
        Message = message;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
        ReadAt = null;

        Validate();
    }

    private void Validate()
    {
        if (RecipientId == 0) throw new ArgumentException("RecipientId must be greater than 0");
        if (RelatedEntityId == 0) throw new ArgumentException("RelatedEntityId must be greater than 0");
        if (string.IsNullOrWhiteSpace(Message)) throw new ArgumentException("Message is required");
        if (Message.Length > 500) throw new ArgumentException("Message cannot exceed 500 characters");
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}