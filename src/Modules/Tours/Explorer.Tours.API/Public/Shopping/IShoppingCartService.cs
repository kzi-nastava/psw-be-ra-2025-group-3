using Explorer.Tours.API.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Shopping
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetMyCart(long touristId);
        ShoppingCartDto AddToCart(long touristId, long tourId);
        ShoppingCartDto RemoveFromCart(long touristId, long tourId);
    }
}
