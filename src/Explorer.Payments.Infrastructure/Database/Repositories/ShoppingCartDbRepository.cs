using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly PaymentsContext _context;

        public ShoppingCartDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public ShoppingCart? GetActiveForTourist(long touristId)
        {
            return _context.ShoppingCarts
                .FirstOrDefault(c => c.TouristId == touristId);
        }

        public ShoppingCart Create(ShoppingCart cart)
        {
            _context.ShoppingCarts.Add(cart);
            _context.SaveChanges();
            return cart;
        }

        public ShoppingCart Update(ShoppingCart cart)
        {
            _context.ShoppingCarts.Update(cart);
            _context.SaveChanges();
            return cart;
        }

        //tour-execution kartica
        public bool HasPurchasedTour(long touristId, long tourId)
        {
            var cart = _context.ShoppingCarts
                .FirstOrDefault(c => c.TouristId == touristId);

            if (cart == null)
                return false;

            return cart.Items.Any(item => item.TourId == tourId);
        }
    }
}