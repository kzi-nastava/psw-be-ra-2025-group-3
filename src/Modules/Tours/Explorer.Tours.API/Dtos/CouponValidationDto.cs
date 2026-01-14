namespace Explorer.Tours.API.Dtos;

public class CouponValidationDto
{
    public string Code { get; set; }
    public long TourId { get; set; }
    public List<long>? TourIds { get; set; } // Lista tour ID-jeva iz korpe (opciono)
}
