using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourPurchaseToken : Entity
    {
        public long TouristId { get; private set; }
        public long TourId { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private TourPurchaseToken() { }

        public TourPurchaseToken(long touristId, long tourId, string token)
        {
           // if (touristId <= 0) throw new ArgumentException("Invalid tourist id.");
           // if (tourId <= 0) throw new ArgumentException("Invalid tour id.");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token cannot be empty.");

            TouristId = touristId;
            TourId = tourId;
            Token = token;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
