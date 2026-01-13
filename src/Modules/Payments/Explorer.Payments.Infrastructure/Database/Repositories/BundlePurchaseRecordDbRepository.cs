using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class BundlePurchaseRecordDbRepository : IBundlePurchaseRecordRepository
    {
        private readonly PaymentsContext _context;

        public BundlePurchaseRecordDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public BundlePurchaseRecord Create(BundlePurchaseRecord record)
        {
            _context.BundlePurchaseRecords.Add(record);
            _context.SaveChanges();
            return record;
        }

        public IEnumerable<BundlePurchaseRecord> GetByTouristId(long touristId)
        {
            return _context.BundlePurchaseRecords
                .Where(r => r.TouristId == touristId)
                .OrderByDescending(r => r.PurchasedAt)
                .ToList();
        }

        public BundlePurchaseRecord? GetById(long id)
        {
            return _context.BundlePurchaseRecords.Find(id);
        }

        public List<long> GetPurchasedBundleIdsByTourist(long touristId)
        {
            return _context.BundlePurchaseRecords
                .Where(r => r.TouristId == touristId)
                .Select(r => r.BundleId)
                .Distinct()
                .ToList();
        }
    }
}