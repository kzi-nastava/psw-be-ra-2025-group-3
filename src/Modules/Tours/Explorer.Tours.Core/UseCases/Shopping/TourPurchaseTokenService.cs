using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Shopping;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Shopping
{
    public class TourPurchaseTokenService: ITourPurchaseTokenService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IMapper _mapper;

        public TourPurchaseTokenService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
        }

        public List<TourPurchaseTokenDto> Checkout(long touristId)
        {
            var cart = _cartRepository.GetActiveForTourist(touristId)
                      ?? throw new InvalidOperationException("Cart not found.");

            if (cart.Items.Count == 0)
                throw new InvalidOperationException("Cart is empty.");

            var tokens = new List<TourPurchaseToken>();

            foreach (var item in cart.Items)
            {
                var token = new TourPurchaseToken(
                    touristId,
                    item.TourId,
                    Guid.NewGuid().ToString()
                );

                tokens.Add(_tokenRepository.Create(token));
            }

            cart.Clear();
            _cartRepository.Update(cart);

            return _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
        }

        public List<TourPurchaseTokenDto> GetTokens(long touristId)
        {
            var tokens = _tokenRepository.GetByTouristId(touristId);
            return _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
        }
    }
}
