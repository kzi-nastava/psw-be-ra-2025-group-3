using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourAccessService : ITourAccessService
    {
        private readonly ITourPurchaseTokenRepository _tokenRepository;

        public TourAccessService(ITourPurchaseTokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public bool HasUserPurchased(long touristId, long tourId)
        {
            // Token postoji => tura kupljena
            return _tokenRepository
                .GetByTouristId(touristId)
                .Any(t => t.TourId == tourId);
        }
    }
}
