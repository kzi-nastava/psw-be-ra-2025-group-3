using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
    {
        private readonly PaymentsContext _context;

        public TourPurchaseTokenDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public TourPurchaseToken Create(TourPurchaseToken token)
        {
            _context.TourPurchaseTokens.Add(token);
            _context.SaveChanges();
            return token;
        }

        public IEnumerable<TourPurchaseToken> GetByTouristId(long touristId)
        {
            return _context.TourPurchaseTokens
                .Where(t => t.TouristId == touristId)
                .ToList();
        }

        public TourPurchaseToken? Get(long id)
        {
            return _context.TourPurchaseTokens
                .FirstOrDefault(t => t.Id == id);
        }
    }
}
