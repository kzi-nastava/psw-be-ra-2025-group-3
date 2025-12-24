using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class GeoPoint : ValueObject
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public GeoPoint(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}