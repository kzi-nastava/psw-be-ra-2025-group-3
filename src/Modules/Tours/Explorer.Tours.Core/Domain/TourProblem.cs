using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;

namespace Explorer.Tours.Core.Domain;

public class TourProblem : AggregateRoot
{
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public ProblemCategory Category { get; private set; }
    public ProblemPriority Priority { get; private set; }
    public string Description { get; private set; }
    public DateTime Time { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public long AuthorId { get; private set; }
    public TourProblemStatus Status { get; private set; }
    public string? ResolvedByTouristComment { get; private set; }
    public List<Message> Messages { get; private set; } // Za agregaciju poruka vezanih za problem
    public bool IsHighlighted { get; private set; }
    public DateTime? AdminDeadline { get; private set; }

    public TourProblem()
    {
        Messages = new List<Message>();
    }

    // Konstruktor za kreiranje novog problema
    public TourProblem(long tourId, long touristId, long authorId, ProblemCategory category, ProblemPriority priority, string description, DateTime time)
    {
        // Validacija - dozvoli negativne ID-jeve za testove
        if (tourId == 0)
            throw new ArgumentException("Tour ID must be valid.", nameof(tourId));
        if (touristId == 0)
            throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
        if (authorId == 0)
            throw new ArgumentException("Author ID must be valid.", nameof(authorId));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (description.Length < 10)
            throw new ArgumentException("Description must contain at least 10 characters.", nameof(description));
        if (time > DateTime.UtcNow)
            throw new ArgumentException("Time cannot be in the future.", nameof(time));

        TourId = tourId;
        TouristId = touristId;
        AuthorId = authorId;
        Category = category;
        Priority = priority;
        Description = description;
        Time = time;
        CreatedAt = DateTime.UtcNow;
        Status = TourProblemStatus.Open; // Default status
        Messages = new List<Message>();
        IsHighlighted = false;

    }

    // Metoda za izmenu problema
    public void Update(ProblemCategory category, ProblemPriority priority, string description, DateTime time)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        if (description.Length < 10)
            throw new ArgumentException("Description must contain at least 10 characters.", nameof(description));

        // Konvertuj time u UTC ako nije
        var utcTime = time.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(time, DateTimeKind.Utc)
            : time.ToUniversalTime();

        if (utcTime > DateTime.UtcNow)
            throw new ArgumentException("Time cannot be in the future.", nameof(time));

        Category = category;
        Priority = priority;
        Description = description;
        Time = utcTime;
        UpdatedAt = DateTime.UtcNow;
    }

    // Podtask 1
    public void MarkAsResolved(string touristComment)
    {
        if (Status == TourProblemStatus.Resolved)
            throw new InvalidOperationException("Problem is already marked as resolved.");

        if (string.IsNullOrWhiteSpace(touristComment))
            throw new ArgumentException("Tourist comment cannot be empty when marking as resolved.", nameof(touristComment));

        Status = TourProblemStatus.Resolved;
        ResolvedByTouristComment = touristComment.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnresolved(string touristComment)
    {
        if (string.IsNullOrWhiteSpace(touristComment))
            throw new ArgumentException("Tourist comment cannot be empty when marking as unresolved.", nameof(touristComment));

        Status = TourProblemStatus.Unresolved;
        ResolvedByTouristComment = touristComment.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMessage(long authorId, string content, AuthorType authorType)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.", nameof(content));

        if (content.Length > 1000)
            throw new ArgumentException("Message content cannot exceed 1000 characters.", nameof(content));

        // Validacija da samo relevantne osobe mogu slati poruke
        if (authorType == AuthorType.Tourist && authorId != TouristId)
            throw new InvalidOperationException("Only the tourist who reported the problem can send messages as Tourist.");

        if (authorType == AuthorType.Author && authorId != AuthorId)
            throw new InvalidOperationException("Only the tour author can send messages as Author.");

        var message = new Message(authorId, content.Trim(), authorType);
        Messages.Add(message);


    }
}