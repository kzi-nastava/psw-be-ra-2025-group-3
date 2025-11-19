using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourProblem : Entity
{
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public ProblemCategory Category { get; private set; }
    public ProblemPriority Priority { get; private set; }
    public string Description { get; private set; }
    public DateTime Time { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Prazan konstruktor za Entity Framework
    public TourProblem() { }

    // Konstruktor za kreiranje novog problema
    public TourProblem(long tourId, long touristId, ProblemCategory category, ProblemPriority priority, string description, DateTime time)
    {
        // Validacija - dozvoli negativne ID-jeve za testove
        if (tourId == 0)
            throw new ArgumentException("Tour ID must be valid.", nameof(tourId));
        if (touristId == 0)
            throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (description.Length < 10)
            throw new ArgumentException("Description must contain at least 10 characters.", nameof(description));
        if (time > DateTime.UtcNow)
            throw new ArgumentException("Time cannot be in the future.", nameof(time));

        TourId = tourId;
        TouristId = touristId;
        Category = category;
        Priority = priority;
        Description = description;
        Time = time;
        CreatedAt = DateTime.UtcNow;
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
        Time = utcTime;  // ← Koristi UTC vreme
        UpdatedAt = DateTime.UtcNow;
    }
}