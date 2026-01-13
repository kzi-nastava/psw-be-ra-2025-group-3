using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class ShoppingCartItemDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; } = "";
        public decimal Price { get; set; }
    }

    public class ShoppingCartDto
    {
        public long TouristId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; } = new();
        public List<BundleOrderItemDto> BundleItems { get; set; } = new();
    }
    public class BundleOrderItemDto
    {
        public long BundleId { get; set; }
        public string BundleName { get; set; }
        public decimal Price { get; set; }
        public int TourCount { get; set; }
    }
}