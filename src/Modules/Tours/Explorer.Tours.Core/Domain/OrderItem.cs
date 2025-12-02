using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class OrderItem : ValueObject
    {
        public long TourId { get; }
        public string TourName { get; }
        public decimal Price { get; }

        [JsonConstructor]
        public OrderItem(long tourId, string tourName, decimal price)
        {
            if (tourId == 0) throw new ArgumentException("Invalid tour id.", nameof(tourId));
            if (string.IsNullOrWhiteSpace(tourName)) throw new ArgumentException("Tour name is required.", nameof(tourName));
            if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));

            TourId = tourId;
            TourName = tourName.Trim();
            Price = price;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return TourId;
            yield return TourName;
            yield return Price;
        }
    }
}