using Explorer.Payments.API.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public.Shopping
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetMyCart(long touristId);
        ShoppingCartDto AddToCart(long touristId, long tourId);
        ShoppingCartDto RemoveFromCart(long touristId, long tourId);
        bool HasPurchasedTour(long touristId, long tourId); //tour-execution kartica
        ShoppingCartDto AddBundleToCart(long touristId, long bundleId);
        ShoppingCartDto RemoveBundleFromCart(long touristId, long bundleId);
        bool HasPurchasedBundle(long touristId, long bundleId);
    }
}
