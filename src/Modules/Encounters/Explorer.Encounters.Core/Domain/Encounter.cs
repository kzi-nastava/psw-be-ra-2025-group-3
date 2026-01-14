using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class Encounter : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public GeoPoint Location { get; private set; }
    public int XP { get; private set; }
    public EncounterStatus Status { get; private set; }
    public EncounterType Type { get; private set; }

    // Misc Encounter
    public string? ActionDescription { get; private set; }

    // Social Encounter
    public int? RequiredPeopleCount { get; private set; }
    public double? RangeInMeters { get; private set; }

    // Hidden Location Encounter
    public string? ImageUrl { get; private set; }

    protected Encounter() { }

    // Constructor za Misc - ima string actionDescription
    public Encounter(string name, string description, GeoPoint location, int xp, EncounterStatus status, string actionDescription)
    {
        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = EncounterType.Misc;
        Status = status;
        ActionDescription = actionDescription;
        Validate();
    }

    // Constructor za Social - ima int i double
    public Encounter(string name, string description, GeoPoint location, int xp, EncounterStatus status, int requiredPeopleCount, double rangeInMeters)
    {
        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = EncounterType.Social;
        Status = status;
        RequiredPeopleCount = requiredPeopleCount;
        RangeInMeters = rangeInMeters;
        Validate();
    }

    // Constructor za Hidden Location - ima bool dummy parametar da se razlikuje od Misc
    public Encounter(string name, string description, GeoPoint location, int xp, EncounterStatus status, string imageUrl, bool isHiddenLocation)
    {
        if (!isHiddenLocation)
            throw new ArgumentException("This constructor is for Hidden Location encounters only.");

        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = EncounterType.HiddenLocation;
        Status = status;
        ImageUrl = imageUrl;
        Validate();
    }

    public void Update(string name, string description, GeoPoint location, int xp, EncounterType type, EncounterStatus status,
                       string? actionDescription = null,
                       int? requiredPeopleCount = null, double? rangeInMeters = null,
                       string? imageUrl = null)
    {
        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = type;
        Status = status;

        ActionDescription = actionDescription;
        RequiredPeopleCount = requiredPeopleCount;
        RangeInMeters = rangeInMeters;
        ImageUrl = imageUrl;

        Validate();
    }

    public void Activate()
    {
        if (Status == EncounterStatus.Active)
            throw new InvalidOperationException("Encounter is already active.");
        Status = EncounterStatus.Active;
    }

    public void Archive()
    {
        if (Status == EncounterStatus.Archived)
            throw new InvalidOperationException("Encounter is already archived.");
        Status = EncounterStatus.Archived;
    }

    protected virtual void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty.");
        if (XP < 0)
            throw new ArgumentException("XP cannot be negative.");
        if (Location == null)
            throw new ArgumentException("Location is required.");

        if (Type == EncounterType.Misc && string.IsNullOrWhiteSpace(ActionDescription))
            throw new ArgumentException("ActionDescription is required for Misc encounters.");

        if (Type == EncounterType.Social)
        {
            if (!RequiredPeopleCount.HasValue || RequiredPeopleCount <= 0)
                throw new ArgumentException("RequiredPeopleCount must be greater than 0 for Social encounters.");
            if (!RangeInMeters.HasValue || RangeInMeters <= 0)
                throw new ArgumentException("RangeInMeters must be greater than 0 for Social encounters.");
        }

        if (Type == EncounterType.HiddenLocation)
        {
            if (string.IsNullOrWhiteSpace(ImageUrl))
                throw new ArgumentException("ImageUrl is required for Hidden Location encounters.");
        }
    }

    public void SetStatus(EncounterStatus status)
    {
        Status = status;
    }
}