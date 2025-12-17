using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class ShoppingCart : AggregateRoot
    {
        public long TouristId { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public decimal TotalPrice { get; private set; }

        private ShoppingCart() { }

        public ShoppingCart(long touristId)
        {
            if (touristId == 0)
                throw new ArgumentException("Invalid tourist id.", nameof(touristId));

            TouristId = touristId;
            Items = new();
            TotalPrice = 0;
        }

        public void AddItem(Tour tour)
        {
            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("Tour must be published to be added to cart.");

            if (Items.Any(i => i.TourId == tour.Id))
                throw new InvalidOperationException("Tour is already in the shopping cart.");

            var item = new OrderItem(tour.Id, tour.Name, tour.Price);
            Items.Add(item);
            RecalculateTotal();
        }

        public void RemoveItem(long tourId)
        {
            var existing = Items.SingleOrDefault(i => i.TourId == tourId);
            if (existing == null) return;

            Items.Remove(existing);
            RecalculateTotal();
        }
        public void Clear()
        {
            Items.Clear();
            TotalPrice = 0;
        }
        private void RecalculateTotal()
        {
            TotalPrice = Items.Sum(i => i.Price);
        }
    }
}