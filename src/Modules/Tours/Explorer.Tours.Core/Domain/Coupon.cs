using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Coupon : AggregateRoot
{
    public string Code { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public long? TourId { get; private set; } // null = valid for all tours of the author
    public long AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Prazan konstruktor za Entity Framework
    public Coupon() { }

    // Konstruktor za kreiranje novog kupona
    public Coupon(string code, decimal discountPercentage, long authorId, DateTime? expiryDate = null, long? tourId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Coupon code cannot be empty.", nameof(code));
        
        if (discountPercentage <= 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 1 and 100.", nameof(discountPercentage));
        
        if (authorId == 0)
            throw new ArgumentException("Author ID must be valid.", nameof(authorId));

        if (expiryDate.HasValue && expiryDate.Value < DateTime.UtcNow)
            throw new ArgumentException("Expiry date cannot be in the past.", nameof(expiryDate));

        Code = code;
        DiscountPercentage = discountPercentage;
        AuthorId = authorId;
        ExpiryDate = expiryDate;
        TourId = tourId;
        CreatedAt = DateTime.UtcNow;
    }

    // Metoda za izmenu kupona
    public void Update(decimal discountPercentage, DateTime? expiryDate = null, long? tourId = null)
    {
        if (discountPercentage <= 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 1 and 100.", nameof(discountPercentage));

        if (expiryDate.HasValue && expiryDate.Value < DateTime.UtcNow)
            throw new ArgumentException("Expiry date cannot be in the past.", nameof(expiryDate));

        DiscountPercentage = discountPercentage;
        ExpiryDate = expiryDate;
        TourId = tourId;
    }

    // Metoda za proveru da li je kupon validan
    public bool IsValid()
    {
        if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow)
            return false;
        
        return true;
    }

    // Metoda za proveru da li kupon važi za određenu turu
    public bool IsValidForTour(long tourId, long tourAuthorId)
    {
        if (!IsValid())
            return false;

        // Kupon mora biti od istog autora
        if (AuthorId != tourAuthorId)
            return false;

        // Ako je TourId null, kupon važi za sve ture autora
        if (TourId == null)
            return true;

        // Inače, mora biti tačno za tu turu
        return TourId == tourId;
    }

    // Metoda za izračunavanje popusta
    public decimal CalculateDiscount(decimal originalPrice)
    {
        if (!IsValid())
            return 0;

        return originalPrice * (DiscountPercentage / 100);
    }
}
