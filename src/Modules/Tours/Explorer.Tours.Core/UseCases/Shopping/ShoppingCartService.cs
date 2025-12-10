using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Shopping
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public ShoppingCartDto GetMyCart(long touristId)
        {
            var cart = _shoppingCartRepository.GetActiveForTourist(touristId)
                       ?? _shoppingCartRepository.Create(new ShoppingCart(touristId));

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto AddToCart(long touristId, long tourId)
        {
            var cart = _shoppingCartRepository.GetActiveForTourist(touristId)
                       ?? _shoppingCartRepository.Create(new ShoppingCart(touristId));

            var tour = _tourRepository.GetById(tourId)
                   ?? throw new InvalidOperationException("Tour not found.");

            cart.AddItem(tour);

            _shoppingCartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto RemoveFromCart(long touristId, long tourId)
        {
            var cart = _shoppingCartRepository.GetActiveForTourist(touristId)
                       ?? _shoppingCartRepository.Create(new ShoppingCart(touristId));

            cart.RemoveItem(tourId);

            _shoppingCartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        //tour-execution kartica
        public bool HasPurchasedTour(long touristId, long tourId)
        {
            return _shoppingCartRepository.HasPurchasedTour(touristId, tourId);
        }
    }
}