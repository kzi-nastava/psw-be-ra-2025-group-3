using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Tourist : Entity
{
    public long PersonId { get; init; } // Foreign key na Person
    public List<long> EquipmentIds { get; set; }
    public int XP { get; private set; }
    public int Level { get; private set; }

    // Konstruktor za EF
    public Tourist(long personId)
    {
        if (personId == 0) throw new ArgumentException("Invalid PersonId");

        PersonId = personId;
        EquipmentIds = new List<long>();
        XP = 0;
        Level = 1;
    }

    // Navigation property (opciono)
    public Person Person { get; init; }

    public void IncreaseXP(int xp)
    {
        if (xp < 0) throw new ArgumentException("XP to add cannot be negative.");
        XP += xp;

        const int xpPerLevel = 100;
        var newLevel = (XP / xpPerLevel) + 1;

        if (newLevel > Level)
            Level = newLevel;
    }
}