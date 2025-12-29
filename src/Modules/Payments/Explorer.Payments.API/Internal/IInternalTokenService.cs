using Explorer.Payments.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface IInternalTokenService
    {
        List<TourPurchaseTokenDto> GetTokens(long touristId);
    }
}
