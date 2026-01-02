using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Shopping
{
    public interface ITourPurchaseTokenService
    {
        List<TourPurchaseTokenDto> Checkout(long touristId);
        List<TourPurchaseTokenDto> GetTokens(long touristId);
    }
}
