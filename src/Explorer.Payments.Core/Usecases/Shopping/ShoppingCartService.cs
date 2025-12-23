using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Core.UseCases.Shopping
{
    public class ShoppingCartService : IShoppingCartService, IInternalShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        //private readonly ITourRepository _tourRepository;
        private readonly IInternalTourService _internalTourService;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            //ITourRepository tourRepository,
            IInternalTourService internalTourService,
            IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            //_tourRepository = tourRepository;
            _internalTourService = internalTourService;
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

            /*var tour = _tourRepository.GetById(tourId)
                   ?? throw new InvalidOperationException("Tour not found.");
            if (tour.ArchivedAt != null)
                throw new InvalidOperationException("Cannot purchase archived tour.");

            cart.AddItem(tour);*/

            var tour = _internalTourService.GetById(tourId) ?? throw new InvalidOperationException("Tour not found.");

            if (tour.ArchivedAt != null)
                throw new InvalidOperationException("Cannot purchase archived tour.");

            if (tour.Status != (int)TourStatusDto.Published)
                throw new InvalidOperationException("Tour must be published to be added to cart.");


            var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);
            cart.AddItem(orderItem);

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