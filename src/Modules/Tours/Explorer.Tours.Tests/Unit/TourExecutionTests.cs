using System;
using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit;

public class TourExecutionTests
{
    [Theory]
    [InlineData(21, 2, 45.2500, 19.8300)]
    [InlineData(-21, -2, 45.2500, 19.8300)]
    public void Creates_valid_tour_execution(long touristId, long tourId, double latitude, double longitude)
    {
        var execution = new TourExecution(touristId, tourId, latitude, longitude);

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
    [InlineData(0, 2, 45.2500, 19.8300)]
    [InlineData(21, 0, 45.2500, 19.8300)]
    [InlineData(21, 2, -91, 19.8300)]
    [InlineData(21, 2, 91, 19.8300)]
    [InlineData(21, 2, 45.2500, -181)]
    [InlineData(21, 2, 45.2500, 181)]
    public void Fails_to_create_with_invalid_data(long touristId, long tourId, double lat, double lng)
    {
        Should.Throw<ArgumentException>(() => new TourExecution(touristId, tourId, lat, lng));
    }

    [Fact]
    public void Completes_active_tour_execution()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        execution.Complete();

        execution.Status.ShouldBe(TourExecutionStatus.Completed);
        execution.CompletionTime.ShouldNotBeNull();
        execution.AbandonTime.ShouldBeNull();
    }

    [Fact]
    public void Abandons_active_tour_execution()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);

        execution.Abandon();

        execution.Status.ShouldBe(TourExecutionStatus.Abandoned);
        execution.AbandonTime.ShouldNotBeNull();
        execution.CompletionTime.ShouldBeNull();
    }

    [Fact]
    public void Fails_to_complete_non_active_tour()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);
        execution.Complete();

        Should.Throw<InvalidOperationException>(() => execution.Complete())
            .Message.ShouldContain("not active");
    }

    [Fact]
    public void Fails_to_abandon_non_active_tour()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);
        execution.Abandon();

        Should.Throw<InvalidOperationException>(() => execution.Abandon())
            .Message.ShouldContain("not active");
    }

    [Fact]
    public void Cannot_complete_after_abandon()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);
        execution.Abandon();

        Should.Throw<InvalidOperationException>(() => execution.Complete());
    }

    [Fact]
    public void Cannot_abandon_after_complete()
    {
        var execution = new TourExecution(21, 2, 45.2500, 19.8300);
        execution.Complete();

        Should.Throw<InvalidOperationException>(() => execution.Abandon());
    }
}