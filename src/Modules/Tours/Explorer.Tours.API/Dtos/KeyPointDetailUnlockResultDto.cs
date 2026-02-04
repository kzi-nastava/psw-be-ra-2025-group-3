namespace Explorer.Tours.API.Dtos;

public class KeyPointDetailUnlockResultDto
{
    public long KeyPointId { get; set; }
    public string Secret { get; set; } = string.Empty;
    public int CostAc { get; set; }
    public int NewBalanceAc { get; set; }
}
