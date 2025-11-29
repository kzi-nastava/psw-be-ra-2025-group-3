using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Monument : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Year { get; private set; }
        public MonumentStatus Status { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }


        public Monument(string name, string description, int year, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid Name.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Invalid Description.");

            if (year == 0)
                throw new ArgumentException("Year 0 does not exist. Use negative numbers for BC dates.");
            if(year > DateTime.Now.Year)
                throw new ArgumentException($"Year must be less than or equal to {DateTime.Now.Year}.");

            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.");

            Name = name;
            Description = description;
            Year = year;
            Status = MonumentStatus.Active;   // default Active
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
