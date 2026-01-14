namespace Explorer.Tours.API.Dtos;

public class CouponCreateDto
{
    public decimal DiscountPercentage { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public long? TourId { get; set; } // null = valid for all tours
}
