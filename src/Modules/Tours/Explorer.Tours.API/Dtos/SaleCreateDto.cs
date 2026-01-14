namespace Explorer.Tours.API.Dtos;

public class SaleCreateDto
{
    public List<long> TourIds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountPercentage { get; set; }
}
