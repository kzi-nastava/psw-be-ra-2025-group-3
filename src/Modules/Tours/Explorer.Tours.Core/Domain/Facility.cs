using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using global::Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
   

   

    public class Facility : Entity
    {
        public string Name { get; protected set; }
        public double Latitude { get; protected set; }
        public double Longitude { get; protected set; }
        public FacilityCategory Category { get; protected set; }

        public Facility() { }

        public Facility(string name, double latitude, double longitude, FacilityCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Category = category;
        }


        public void Update(string name, double latitude, double longitude, FacilityCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Category = category;
        }
    }
}
