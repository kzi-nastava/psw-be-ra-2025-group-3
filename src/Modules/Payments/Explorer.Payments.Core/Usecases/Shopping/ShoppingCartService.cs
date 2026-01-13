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
        private readonly IInternalBundleService _internalBundleService;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            //ITourRepository tourRepository,
            IInternalTourService internalTourService,
            IInternalBundleService internalBundleService,
            IMapper mapper)
        {
            _shoppingCartRepository = shoppingCartRepository;
            //_tourRepository = tourRepository;
            _internalTourService = internalTourService;
            _internalBundleService = internalBundleService;
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

            var tour = _internalTourService.GetById(tourId);

            if (tour == null)
                throw new InvalidOperationException("Tour not found.");

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
        public ShoppingCartDto AddBundleToCart(long touristId, long bundleId)
        {
            Console.WriteLine($"\n[AddBundleToCart] START - Tourist: {touristId}, Bundle: {bundleId}");

            // ✅ PROVERI DA LI JE VEĆ KUPLJEN
            var alreadyPurchased = _shoppingCartRepository.HasPurchasedBundle(touristId, bundleId);
            if (alreadyPurchased)
            {
                Console.WriteLine($"[AddBundleToCart ERROR] Bundle {bundleId} already purchased!");
                throw new InvalidOperationException("You have already purchased this bundle.");
            }

            var cart = _shoppingCartRepository.GetActiveForTourist(touristId)
                       ?? _shoppingCartRepository.Create(new ShoppingCart(touristId));

            Console.WriteLine($"[AddBundleToCart] Cart loaded - Items: {cart.Items.Count}, BundleItems: {cart.BundleItems.Count}");

            var bundle = _internalBundleService.GetById(bundleId);
            if (bundle == null)
            {
                Console.WriteLine($"[AddBundleToCart ERROR] Bundle {bundleId} not found!");
                throw new InvalidOperationException("Bundle not found.");
            }

            Console.WriteLine($"[AddBundleToCart] Bundle found: {bundle.Name}, Status: {bundle.Status}");

            if (bundle.Status != 1)
            {
                Console.WriteLine($"[AddBundleToCart ERROR] Bundle not published! Status: {bundle.Status}");
                throw new InvalidOperationException("Bundle must be published to be added to cart.");
            }

            var bundleItem = new BundleOrderItem(
                bundle.Id,
                bundle.Name,
                bundle.Price,
                bundle.TourIds.Count
            );

            Console.WriteLine($"[AddBundleToCart] Creating BundleOrderItem");

            cart.AddBundleItem(bundleItem);
            Console.WriteLine($"[AddBundleToCart] After AddBundleItem - BundleItems: {cart.BundleItems.Count}");

            _shoppingCartRepository.Update(cart);
            Console.WriteLine($"[AddBundleToCart] Cart updated in DB");

            var result = _mapper.Map<ShoppingCartDto>(cart);
            Console.WriteLine($"[AddBundleToCart] END\n");

            return result;
        }
        public ShoppingCartDto RemoveBundleFromCart(long touristId, long bundleId)
        {
            var cart = _shoppingCartRepository.GetActiveForTourist(touristId)
                       ?? _shoppingCartRepository.Create(new ShoppingCart(touristId));

            cart.RemoveBundleItem(bundleId);
            _shoppingCartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }
        public bool HasPurchasedBundle(long touristId, long bundleId)
        {
            return _shoppingCartRepository.HasPurchasedBundle(touristId, bundleId);
        }
    }
}