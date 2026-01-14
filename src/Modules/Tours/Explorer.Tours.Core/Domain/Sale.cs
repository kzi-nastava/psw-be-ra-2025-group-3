using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Sale : AggregateRoot
{
    public List<long> TourIds { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public long AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Sale()
    {
        TourIds = new List<long>();
    }

    public Sale(List<long> tourIds, DateTime startDate, DateTime endDate, decimal discountPercentage, long authorId)
    {
        if (tourIds == null || tourIds.Count == 0)
            throw new ArgumentException("Sale must contain at least one tour.");
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.");
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        
        var maxEndDate = startDate.AddDays(14); // Maximum 2 weeks
        if (endDate > maxEndDate)
            throw new ArgumentException("End date cannot be more than 2 weeks from start date.");

        TourIds = tourIds;
        StartDate = startDate;
        EndDate = endDate;
        DiscountPercentage = discountPercentage;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(List<long> tourIds, DateTime startDate, DateTime endDate, decimal discountPercentage)
    {
        if (tourIds == null || tourIds.Count == 0)
            throw new ArgumentException("Sale must contain at least one tour.");
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.");
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        
        var maxEndDate = startDate.AddDays(14); // Maximum 2 weeks
        if (endDate > maxEndDate)
            throw new ArgumentException("End date cannot be more than 2 weeks from start date.");

        TourIds = tourIds;
        StartDate = startDate;
        EndDate = endDate;
        DiscountPercentage = discountPercentage;
        UpdatedAt = DateTime.UtcNow;
    }
}
