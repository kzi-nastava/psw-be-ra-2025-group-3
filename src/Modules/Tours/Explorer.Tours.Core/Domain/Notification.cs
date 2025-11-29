using System;
using Explorer.BuildingBlocks.Core.Domain;
namespace Explorer.Tours.Core.Domain;
public class Notification : Entity
{
    public long RecipientId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Content { get; private set; }
    public long RelatedProblemId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime Timestamp { get; private set; }

    private Notification() { }

    // Konstruktor za kreiranje notifikacije
    public Notification(long recipientId, NotificationType type, string content, long relatedProblemId)
    {
        if (recipientId == 0)
            throw new ArgumentException("Recipient ID must be valid.", nameof(recipientId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        if (relatedProblemId == 0)
            throw new ArgumentException("Related problem ID must be valid.", nameof(relatedProblemId));

        RecipientId = recipientId;
        Type = type;
        Content = content.Trim();
        RelatedProblemId = relatedProblemId;
        IsRead = false;
        Timestamp = DateTime.UtcNow;
    }
}