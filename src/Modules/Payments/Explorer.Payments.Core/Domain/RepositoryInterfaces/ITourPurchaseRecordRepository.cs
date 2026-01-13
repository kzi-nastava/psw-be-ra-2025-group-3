
namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseRecordRepository
    {
        TourPurchaseRecord Create(TourPurchaseRecord record);
        IEnumerable<TourPurchaseRecord> GetByTouristId(long touristId);
    }
}