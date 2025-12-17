using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class KeyPoint : Entity
    {
        public long TourId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ImageUrl { get; private set; }
        public string Secret { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private KeyPoint() { }

        public KeyPoint(long tourId, string name, string description, string imageUrl, string secret, double latitude, double longitude)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be valid.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.");
            TourId = tourId;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Secret = secret;
            Latitude = latitude;
            Longitude = longitude;
        }
        public void Update(string name, string description, string imageUrl, string secret,
                   double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.");

            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Secret = secret;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}