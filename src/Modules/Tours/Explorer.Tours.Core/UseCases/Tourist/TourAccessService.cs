using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourAccessService : ITourAccessService
    {
        //private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IInternalTokenService _tokenService;
        /*
        public TourAccessService(ITourPurchaseTokenRepository tokenRepository)
        {
           // _tokenRepository = tokenRepository;
        }
        */
        public TourAccessService(IInternalTokenService tokenService)
        {
            _tokenService = tokenService;
        }
        
        
        public bool HasUserPurchased(long touristId, long tourId)
        {
            // Token postoji => tura kupljena
            /*return _tokenRepository
                .GetByTouristId(touristId)
                .Any(t => t.TourId == tourId);
            */
            
            return _tokenService
                .GetTokens(touristId)
                .Any(t => t.TourId == tourId);
        }
    }
}
