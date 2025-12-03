using Explorer.Tours.API.Public.Review;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Review;

[Collection("Sequential")]
public class TourReviewQueryTests : BaseToursIntegrationTest
{
    public TourReviewQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_all_reviews_for_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var reviews = service.GetReviewsForTour(-2);

        reviews.ShouldNotBeEmpty();
        reviews.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Gets_my_review()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var review = service.GetMyReview(-2, -25); 

        review.ShouldNotBeNull();
        review.TouristId.ShouldBe(-25); 
        review.TourId.ShouldBe(-2);
        review.Rating.ShouldBe(4);
    }

    [Fact]
    public void Returns_null_when_no_review()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

        var review = service.GetMyReview(-2, -26); 

        review.ShouldBeNull();
    }
}