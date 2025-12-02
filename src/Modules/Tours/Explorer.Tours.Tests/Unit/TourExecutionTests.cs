using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit;

public class TourExecutionTests
{
    [Theory]
    [InlineData(21, 2, 45.2500, 19.8300)]      // Pozitivni ID-evi
    [InlineData(-21, -2, 45.2500, 19.8300)]    // Negativni ID-evi (testni podaci)
    public void Creates_valid_tour_execution(long touristId, long tourId, double latitude, double longitude)
    {
        // Act
        var execution = new TourExecution(touristId, tourId, latitude, longitude);

        // Assert
        execution.ShouldNotBeNull();
        execution.TouristId.ShouldBe(touristId);
        execution.TourId.ShouldBe(tourId);
        execution.Status.ShouldBe(TourExecutionStatus.Active);
        execution.StartLatitude.ShouldBe(latitude);
        execution.StartLongitude.ShouldBe(longitude);
        execution.StartTime.ShouldNotBe(default(DateTime));
        execution.CompletionTime.ShouldBeNull();
        execution.AbandonTime.ShouldBeNull();
    }

    [Theory]
    [InlineData(0, 2, 45.2500, 19.8300)]       // Invalid touristId = 0
    [InlineData(21, 0, 45.2500, 19.8300)]      // Invalid tourId = 0
    [InlineData(21, 2, -91, 19.8300)]          // Latitude too low
    [InlineData(21, 2, 91, 19.8300)]           // Latitude too high
    [InlineData(21, 2, 45.2500, -181)]         // Longitude too low
    [InlineData(21, 2, 45.2500, 181)]          // Longitude too high
    public void Fails_to_create_with_invalid_data(long touristId, long tourId, double lat, double lng)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new TourExecution(touristId, tourId, lat, lng));
    }
}