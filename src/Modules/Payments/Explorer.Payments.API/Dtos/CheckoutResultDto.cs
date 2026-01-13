
namespace Explorer.Payments.API.Dtos
{
    public class CheckoutResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public List<TourPurchaseTokenDto> Tokens { get; set; } = new();
        public List<TourPurchaseRecordDto> PurchaseRecords { get; set; } = new();
        public List<BundlePurchaseRecordDto> BundlePurchaseRecords { get; set; }
    }
}