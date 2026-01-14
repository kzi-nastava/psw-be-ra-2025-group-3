namespace Explorer.Tours.API.Dtos;

public class CouponDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public long? TourId { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
}
