using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class ShoppingCart : AggregateRoot
    {
        public long TouristId { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public List<BundleOrderItem> BundleItems { get; private set; }
        public decimal TotalPrice { get; private set; }

        private ShoppingCart() { }

        public ShoppingCart(long touristId)
        {
            if (touristId == 0)
                throw new ArgumentException("Invalid tourist id.", nameof(touristId));

            TouristId = touristId;
            Items = new();
            BundleItems = new();
            TotalPrice = 0;
        }
        /*
        public void AddItem(Tour tour)
        {
            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("Tour must be published to be added to cart.");

            if (Items.Any(i => i.TourId == tour.Id))
                throw new InvalidOperationException("Tour is already in the shopping cart.");

            var item = new OrderItem(tour.Id, tour.Name, tour.Price);
            Items.Add(item);
            RecalculateTotal();
        }*/
        
        
        public void AddItem(OrderItem orderItem)
        {
            

            if (Items.Any(i => i.TourId == orderItem.TourId))
                throw new InvalidOperationException("Tour is already in the shopping cart.");
            
            Items.Add(orderItem); 
            RecalculateTotal(); 
        }
        public void AddBundleItem(BundleOrderItem bundleItem)
        {
            if (BundleItems.Any(i => i.BundleId == bundleItem.BundleId))
                throw new InvalidOperationException("Bundle is already in the shopping cart.");

            BundleItems.Add(bundleItem);
            RecalculateTotal();
        }

        public void RemoveItem(long tourId)
        {
            var existing = Items.SingleOrDefault(i => i.TourId == tourId);
            if (existing == null) return;

            Items.Remove(existing);
            RecalculateTotal();
        }
        public void ClearItems()
        {
            Items.Clear();
            BundleItems.Clear();
            TotalPrice = 0;
        }
        // za dugme
        public void Clear()
        {
            Items = new List<OrderItem>();
            BundleItems = new List<BundleOrderItem>();
            TotalPrice = 0;
        }
        private void RecalculateTotal()
        {
            TotalPrice = Items.Sum(i => i.Price) + BundleItems.Sum(b => b.Price);
        }
        public void RemoveBundleItem(long bundleId)
        {
            var existing = BundleItems.SingleOrDefault(i => i.BundleId == bundleId);
            if (existing == null) return;
            BundleItems.Remove(existing);
            RecalculateTotal();
        }
    }
}