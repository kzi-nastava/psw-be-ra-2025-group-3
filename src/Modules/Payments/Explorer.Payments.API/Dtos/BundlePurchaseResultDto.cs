using System.Collections.Generic;

namespace Explorer.Payments.API.Dtos
{
    public class BundlePurchaseResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public BundlePurchaseRecordDto PurchaseRecord { get; set; }
        public List<TourPurchaseTokenDto> Tokens { get; set; }
    }
}