using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
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
    }
}