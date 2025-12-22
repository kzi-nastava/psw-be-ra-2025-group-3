using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Meetup : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DateTime { get; private set; }
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public long CreatorId { get; private set; }
        public long? TourId { get; private set; }

        public Meetup(string title, string description, DateTime dateTime, decimal latitude, decimal longitude, long creatorId, long? tourId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            
            if (dateTime <= DateTime.UtcNow)
                throw new ArgumentException("DateTime cannot be in the past.", nameof(dateTime));
            
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
            
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
            
            if (creatorId == 0)
                throw new ArgumentException("CreatorId must be valid.", nameof(creatorId));

            Title = title;
            Description = description;
            DateTime = dateTime;
            Latitude = latitude;
            Longitude = longitude;
            CreatorId = creatorId;
            TourId = tourId;
        }

        public void Update(string title, string description, DateTime dateTime, decimal latitude, decimal longitude, long? tourId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            
            if (dateTime <= DateTime.UtcNow)
                throw new ArgumentException("DateTime cannot be in the past.", nameof(dateTime));
            
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
            
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));

            Title = title;
            Description = description;
            DateTime = dateTime;
            Latitude = latitude;
            Longitude = longitude;
            TourId = tourId;
        }
    }
}
