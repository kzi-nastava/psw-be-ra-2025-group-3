// src/Modules/Payments/Explorer.Payments.Infrastructure/Database/Repositories/TourPurchaseRecordDbRepository.cs
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class TourPurchaseRecordDbRepository : ITourPurchaseRecordRepository
    {
        private readonly PaymentsContext _context;

        public TourPurchaseRecordDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public TourPurchaseRecord Create(TourPurchaseRecord record)
        {
            _context.TourPurchaseRecords.Add(record);
            _context.SaveChanges();
            return record;
        }

        public IEnumerable<TourPurchaseRecord> GetByTouristId(long touristId)
        {
            return _context.TourPurchaseRecords
                .Where(r => r.TouristId == touristId)
                .OrderByDescending(r => r.PurchasedAt)
                .ToList();
        }
    }
}