
namespace Explorer.Payments.API.Dtos
{
    public class TourPurchaseRecordDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public long TourId { get; set; }
        public decimal PriceAc { get; set; }
        public DateTime PurchasedAt { get; set; }
    }
}