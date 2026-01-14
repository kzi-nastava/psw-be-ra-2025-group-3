namespace Explorer.Tours.API.Dtos;

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public long? AppliedToTourId { get; set; } // ID ture na koju se primenjuje popust
}
