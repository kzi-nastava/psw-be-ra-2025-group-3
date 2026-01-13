using System;

namespace Explorer.Payments.API.Dtos
{
    public class BundlePurchaseRecordDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public long BundleId { get; set; }
        public decimal PriceAc { get; set; }
        public DateTime PurchasedAt { get; set; }
    }
}