using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Bundle : AggregateRoot
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public BundleStatus Status { get; private set; }
        public long AuthorId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }
        public List<long> TourIds { get; private set; }

        private Bundle()
        {
            TourIds = new List<long>();
        }

        public Bundle(string name, decimal price, long authorId, List<long> tourIds)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Bundle name cannot be empty.");
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");
            if (tourIds == null || tourIds.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");

            Name = name;
            Price = price;
            AuthorId = authorId;
            TourIds = tourIds;
            Status = BundleStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, decimal price, List<long> tourIds)
        {
            if (Status == BundleStatus.Archived)
                throw new InvalidOperationException("Cannot modify an archived bundle.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Bundle name cannot be empty.");
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");
            if (tourIds == null || tourIds.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");

            Name = name;
            Price = price;
            TourIds = tourIds;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status == BundleStatus.Published)
                throw new InvalidOperationException("Bundle is already published.");
            if (Status == BundleStatus.Archived)
                throw new InvalidOperationException("Cannot publish an archived bundle.");

            Status = BundleStatus.Published;
            PublishedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != BundleStatus.Published)
                throw new InvalidOperationException("Only published bundles can be archived.");

            Status = BundleStatus.Archived;
            ArchivedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}