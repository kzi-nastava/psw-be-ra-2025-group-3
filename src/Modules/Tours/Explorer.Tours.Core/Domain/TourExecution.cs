using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourExecution : AggregateRoot
{
    public long TouristId { get; private set; }
    public long TourId { get; private set; }
    public DateTime StartTime { get; private set; }
    public TourExecutionStatus Status { get; private set; }
    public double StartLatitude { get; private set; }
    public double StartLongitude { get; private set; }
    public DateTime? CompletionTime { get; private set; }
    public DateTime? AbandonTime { get; private set; }

    private TourExecution() { }

    public TourExecution(long touristId, long tourId, double startLatitude, double startLongitude)
    {
        if (touristId == 0)
            throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
        if (tourId == 0)
            throw new ArgumentException("Tour ID must be valid.", nameof(tourId));
        if (startLatitude < -90 || startLatitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(startLatitude));
        if (startLongitude < -180 || startLongitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(startLongitude));

        TouristId = touristId;
        TourId = tourId;
        StartTime = DateTime.UtcNow;
        Status = TourExecutionStatus.Active;
        StartLatitude = startLatitude;
        StartLongitude = startLongitude;
    }
}