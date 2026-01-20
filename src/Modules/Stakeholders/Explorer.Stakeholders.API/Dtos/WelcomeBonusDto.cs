namespace Explorer.Stakeholders.API.Dtos;

public class WelcomeBonusDto
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public BonusTypeDto BonusType { get; set; }
    public int Value { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}

public enum BonusTypeDto
{
    AC100 = 1,
    AC250 = 2,
    AC500 = 3,
    Discount10 = 4,
    Discount20 = 5,
    Discount30 = 6
}
