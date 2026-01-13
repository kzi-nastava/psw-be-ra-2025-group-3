using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Shopping
{
    public interface IBundlePurchaseService
    {
        BundlePurchaseResultDto PurchaseBundle(long touristId, long bundleId);
        List<long> GetPurchasedBundleIds(long touristId);


    }
}