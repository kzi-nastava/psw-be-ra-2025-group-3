using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Explorer.Payments.Core.Domain
{
    public class BundleOrderItem : ValueObject
    {
        public long BundleId { get; }
        public string BundleName { get; }
        public decimal Price { get; }
        public int TourCount { get; }

        // DODAJ OVO - private parameterless constructor za EF Core i JSON:
        private BundleOrderItem() { }

        [JsonConstructor]
        public BundleOrderItem(long bundleId, string bundleName, decimal price, int tourCount)
        {
            if (bundleId == 0) throw new ArgumentException("Invalid bundle id.", nameof(bundleId));
            if (string.IsNullOrWhiteSpace(bundleName)) throw new ArgumentException("Bundle name is required.", nameof(bundleName));
            if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
            if (tourCount < 1) throw new ArgumentException("Tour count must be at least 1.", nameof(tourCount));

            BundleId = bundleId;
            BundleName = bundleName.Trim();
            Price = price;
            TourCount = tourCount;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return BundleId;
            yield return BundleName;
            yield return Price;
            yield return TourCount;
        }
    }
}