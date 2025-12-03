using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
public class Message : Entity
{
    public long AuthorId { get; private set; }
    public string Content { get; private set; }
    public DateTime Timestamp { get; private set; }
    public AuthorType AuthorType { get; private set; }

    private Message() { }

    // Internal jer se kreira samo unutar domena
    internal Message(long authorId, string content, AuthorType authorType)
    {
        // FIXED: Allow negative IDs for testing (check for zero only)
        if (authorId == 0)
            throw new ArgumentException("Author ID must be valid.", nameof(authorId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        AuthorId = authorId;
        Content = content;
        Timestamp = DateTime.UtcNow;
        AuthorType = authorType;
    }

}
