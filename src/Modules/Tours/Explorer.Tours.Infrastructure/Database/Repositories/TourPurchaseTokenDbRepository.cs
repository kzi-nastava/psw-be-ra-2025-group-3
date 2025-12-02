using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
    {
        private readonly ToursContext _context;

        public TourPurchaseTokenDbRepository(ToursContext context)
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
