using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Payments.Core.Domain
{
    public class BundlePurchaseRecord : Entity
    {
        public long TouristId { get; private set; }
        public long BundleId { get; private set; }
        public decimal PriceAc { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        private BundlePurchaseRecord() { }

        public BundlePurchaseRecord(long touristId, long bundleId, decimal priceAc)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.");
            if (bundleId == 0) throw new ArgumentException("Invalid bundle id.");
            if (priceAc < 0) throw new ArgumentException("Price cannot be negative.");

            TouristId = touristId;
            BundleId = bundleId;
            PriceAc = priceAc;
            PurchasedAt = DateTime.UtcNow;
        }
    }
}