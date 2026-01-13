using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IBundlePurchaseRecordRepository
    {
        BundlePurchaseRecord Create(BundlePurchaseRecord record);
        IEnumerable<BundlePurchaseRecord> GetByTouristId(long touristId);
        BundlePurchaseRecord? GetById(long id);
        List<long> GetPurchasedBundleIdsByTourist(long touristId);

    }
}