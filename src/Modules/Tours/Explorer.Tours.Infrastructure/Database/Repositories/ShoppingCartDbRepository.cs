using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly ToursContext _context;

        public ShoppingCartDbRepository(ToursContext context)
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
    }
}