using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseTokenRepository
    {
        TourPurchaseToken Create(TourPurchaseToken token);
        IEnumerable<TourPurchaseToken> GetByTouristId(long touristId);
        TourPurchaseToken? Get(long id);
    }
}
