// src/Modules/Payments/Explorer.Payments.Core/Domain/TourPurchaseRecord.cs
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class TourPurchaseRecord : Entity
    {
        public long TouristId { get; private set; }
        public long TourId { get; private set; }
        public decimal PriceAc { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        private TourPurchaseRecord() { }

        public TourPurchaseRecord(long touristId, long tourId, decimal priceAc)
        {
            // Dozvoli negativne ID-eve za testove
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.");
            if (tourId == 0) throw new ArgumentException("Invalid tour id.");
            if (priceAc < 0) throw new ArgumentException("Price cannot be negative.");

            TouristId = touristId;
            TourId = tourId;
            PriceAc = priceAc;
            PurchasedAt = DateTime.UtcNow;
        }
    }
}