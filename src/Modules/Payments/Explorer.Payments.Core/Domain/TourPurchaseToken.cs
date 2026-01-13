// src/Modules/Payments/Explorer.Payments.Core/Domain/TourPurchaseToken.cs
using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Payments.Core.Domain
{
    public class TourPurchaseToken : Entity
    {
        public long TouristId { get; private set; }
        public long TourId { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF Core private constructor
        private TourPurchaseToken() { }

        // GLAVNI konstruktor - koristi se u production kodu
        public TourPurchaseToken(long touristId, long tourId)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.");
            if (tourId == 0) throw new ArgumentException("Invalid tour id.");

            TouristId = touristId;
            TourId = tourId;
            Token = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}