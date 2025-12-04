using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json.Serialization;

namespace Explorer.Tours.Core.Domain
{
    public class TourDuration : ValueObject
    {
        public int TimeInMinutes { get; private set; }
        public TransportType Transportation { get; private set; }

        [JsonConstructor]
        public TourDuration(int timeInMinutes, TransportType transportation)
        {
            TimeInMinutes = timeInMinutes;
            Transportation = transportation;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TimeInMinutes;
            yield return Transportation;
        }
    }
}