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

    public DateTime LastActivity { get; private set; }//task2
    public List<KeyPointCompletion> CompletedKeyPoints { get; private set; }//task2

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
        LastActivity = DateTime.UtcNow; // task2
        CompletedKeyPoints = new List<KeyPointCompletion>();//task2
    }

    //metode za task2
    public bool CheckLocationProgress(double currentLatitude, double currentLongitude, List<KeyPoint> tourKeyPoints)
    {
        LastActivity = DateTime.UtcNow; // Ažuriraj LastActivity

        foreach (var keyPoint in tourKeyPoints)
        {
            // Proveri da li je već kompletirana
            if (CompletedKeyPoints.Any(c => c.KeyPointId == keyPoint.Id))
                continue;

            // Proveri distancu (200 metaraRadius)
            double distance = CalculateDistance(currentLatitude, currentLongitude, keyPoint.Latitude, keyPoint.Longitude);

            if (distance <= 200) // 200 metara
            {
                var completion = new KeyPointCompletion(keyPoint.Id, DateTime.UtcNow);
                CompletedKeyPoints.Add(completion);
                return true; // Kompletirana nova tačka
            }
        }

        return false; // Nije kompletirana nijedna nova tačka
    }

    // Haversine formula za računanje distance između dve GPS koordinate (u metrima)
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Radijus Zemlje u metrima
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}