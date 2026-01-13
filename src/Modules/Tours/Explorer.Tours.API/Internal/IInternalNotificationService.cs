
namespace Explorer.Tours.API.Internal
{
    public interface IInternalNotificationService
    {
        void CreateTourPurchaseNotification(long recipientId, long tourId, string tourName);
        void CreateBundlePurchaseNotification(long touristId, long bundleId, string bundleName);
    }
}