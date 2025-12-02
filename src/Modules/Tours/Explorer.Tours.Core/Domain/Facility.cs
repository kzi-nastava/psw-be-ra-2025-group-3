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
        public long Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public FacilityCategory Category { get; set; }

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
