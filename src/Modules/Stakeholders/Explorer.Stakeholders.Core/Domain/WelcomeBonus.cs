using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum BonusType
{
    AC100 = 1,      // 100 Adventure Coins (30%)
    AC250 = 2,      // 250 Adventure Coins (20%)
    AC500 = 3,      // 500 Adventure Coins (10%)
    Discount10 = 4, // 10% popust (20%)
    Discount20 = 5, // 20% popust (15%)
    Discount30 = 6  // 30% popust (5%)
}

public class WelcomeBonus : Entity
{
    public long PersonId { get; private set; }
    public BonusType BonusType { get; private set; }
    public int Value { get; private set; } // Za AC: iznos, za popust: procenat (10, 20, 30)
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    private WelcomeBonus() { }

    public WelcomeBonus(long personId, BonusType bonusType)
    {
        if (personId == 0) throw new ArgumentException("Invalid personId.");
        
        PersonId = personId;
        BonusType = bonusType;
        IsUsed = false;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddDays(30); // Bonus važi 30 dana
        UsedAt = null;

        // Postavi vrednost na osnovu tipa bonusa
        Value = bonusType switch
        {
            BonusType.AC100 => 100,
            BonusType.AC250 => 250,
            BonusType.AC500 => 500,
            BonusType.Discount10 => 10,
            BonusType.Discount20 => 20,
            BonusType.Discount30 => 30,
            _ => throw new ArgumentException("Invalid bonus type.")
        };
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Bonus is already used.");
        
        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("Bonus has expired.");

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    public bool IsActive()
    {
        return !IsUsed && !IsExpired();
    }
}
