namespace Explorer.Tours.API.Dtos;

public class SaleDto
{
    public long Id { get; set; }
    public List<long> TourIds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountPercentage { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
