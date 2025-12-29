using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface IInternalShoppingCartService
    {
        bool HasPurchasedTour(long touristId, long tourId);
    }
}
