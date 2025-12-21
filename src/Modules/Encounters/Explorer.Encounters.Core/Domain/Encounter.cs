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

    private Encounter() { }
    public Encounter(string name, string description, GeoPoint location, int xp, EncounterType type)
    {
        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = type;
        Status = EncounterStatus.Draft;

        Validate();
    }

    public void Update(string name, string description, GeoPoint location, int xp, EncounterType type)
    {
        Name = name;
        Description = description;
        Location = location;
        XP = xp;
        Type = type;

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

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty.");
        if (XP < 0)
            throw new ArgumentException("XP cannot be negative.");
        if (Location == null)
            throw new ArgumentException("Location is required.");
    }
}