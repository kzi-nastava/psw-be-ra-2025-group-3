using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Preference : Entity
{
    public long TouristId { get; private set; }
    public TourDifficulty Difficulty { get; private set; }

    // Ocene za prevozna sredstva (0-3)
    public int WalkingRating { get; private set; }
    public int BicycleRating { get; private set; }
    public int CarRating { get; private set; }
    public int BoatRating { get; private set; }

    public List<string> Tags { get; private set; }

    // Prazan konstruktor za Entity Framework
    public Preference()
    {
        Tags = new List<string>();
    }

    // Konstruktor za kreiranje novih preferenci
    public Preference(
        long touristId,
        TourDifficulty difficulty,
        int walkingRating,
        int bicycleRating,
        int carRating,
        int boatRating,
        List<string>? tags = null)
    {
        if (touristId == 0) // VALIDACIJA
            throw new ArgumentException("Tourist ID cannot be zero.", nameof(touristId));

        if (!IsValidRating(walkingRating))
            throw new ArgumentException("Walking rating must be between 0 and 3.", nameof(walkingRating));
        if (!IsValidRating(bicycleRating))
            throw new ArgumentException("Bicycle rating must be between 0 and 3.", nameof(bicycleRating));
        if (!IsValidRating(carRating))
            throw new ArgumentException("Car rating must be between 0 and 3.", nameof(carRating));
        if (!IsValidRating(boatRating))
            throw new ArgumentException("Boat rating must be between 0 and 3.", nameof(boatRating));

        if (tags == null || tags.Count == 0)
            throw new ArgumentException("At least one tag must be selected.", nameof(tags));

        TouristId = touristId;
        Difficulty = difficulty;
        WalkingRating = walkingRating;
        BicycleRating = bicycleRating;
        CarRating = carRating;
        BoatRating = boatRating;
        Tags = tags ?? new List<string>();
    }

    private bool IsValidRating(int rating) => rating >= 0 && rating <= 3;

    // Metoda za izmenu preferenci
    public void Update(
        TourDifficulty difficulty,
        int walkingRating,
        int bicycleRating,
        int carRating,
        int boatRating,
        List<string>? tags = null)
    {
        if (!IsValidRating(walkingRating))
            throw new ArgumentException("Walking rating must be between 0 and 3.", nameof(walkingRating));
        if (!IsValidRating(bicycleRating))
            throw new ArgumentException("Bicycle rating must be between 0 and 3.", nameof(bicycleRating));
        if (!IsValidRating(carRating))
            throw new ArgumentException("Car rating must be between 0 and 3.", nameof(carRating));
        if (!IsValidRating(boatRating))
            throw new ArgumentException("Boat rating must be between 0 and 3.", nameof(boatRating));

        if (tags == null || tags.Count == 0)
            throw new ArgumentException("At least one tag must be selected.", nameof(tags));

        Difficulty = difficulty;
        WalkingRating = walkingRating;
        BicycleRating = bicycleRating;
        CarRating = carRating;
        BoatRating = boatRating;
        Tags = tags ?? new List<string>();
    }
}