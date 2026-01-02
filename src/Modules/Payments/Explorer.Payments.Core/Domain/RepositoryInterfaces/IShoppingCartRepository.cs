using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart? GetActiveForTourist(long touristId);
        ShoppingCart Create(ShoppingCart cart);
        ShoppingCart Update(ShoppingCart cart);
        bool HasPurchasedTour(long touristId, long tourId); //tour-execution kartica
    }
}