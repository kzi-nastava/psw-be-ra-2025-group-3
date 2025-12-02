using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Tourist : Entity
{
    public long PersonId { get; init; } // Foreign key na Person
    public List<long> EquipmentIds { get; set; }

    // Konstruktor za EF
    public Tourist(long personId)
    {
        if (personId == 0) throw new ArgumentException("Invalid PersonId");

        PersonId = personId;
        EquipmentIds = new List<long>();
    }

    // Navigation property (opciono)
    public Person Person { get; init; }
}