using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Position: Entity
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public long TouristId { get; set; }

        public Position() { }
        public Position(long touristId, double lat, double lng)
        {
            TouristId = touristId;
            Latitude = lat;
            Longitude = lng;
        }

        public void Update(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
    }
}
