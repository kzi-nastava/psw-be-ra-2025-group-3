using Explorer.Tours.API.Public.Review;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Review;

[Collection("Sequential")]
public class TourReviewEligibilityTests : BaseToursIntegrationTest
{
    public TourReviewEligibilityTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Can_review_with_valid_conditions()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var result = service.CheckEligibility(-2, -25); 

        result.CanReview.ShouldBeTrue();
        result.ReasonIfNot.ShouldBeNull();
        result.CurrentProgress.ShouldBe(50.0);
    }

    [Fact]
    public void Cannot_review_with_low_progress()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var result = service.CheckEligibility(-2, -26); 

        result.CanReview.ShouldBeFalse();
        result.ReasonIfNot.ShouldContain("35%");
        result.CurrentProgress.ShouldBe(20.0);
    }

    [Fact]
    public void Cannot_review_after_7_days()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var result = service.CheckEligibility(-2, -27); 

        result.CanReview.ShouldBeFalse();
        result.ReasonIfNot.ShouldContain("7 days");
    }

    [Fact]
    public void Cannot_review_without_tour_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var result = service.CheckEligibility(-2, -99);

        result.CanReview.ShouldBeFalse();
        result.ReasonIfNot.ShouldContain("purchase and start");
    }
}