using Explorer.Tours.API.Public.Review;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
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
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        RemoveReviewIfExists(db, -2, -25);
        // 🔹 Arrange – seed review
        db.TourReviews.Add(new TourReview(
            tourId: -2,
            touristId: -25,
            rating: 4,
            comment: "Good tour",
            progressPercentage: 50
        ));
        db.SaveChanges();

        // 🔹 Act
        var reviews = service.GetReviewsForTour(-2);

        // 🔹 Assert
        reviews.ShouldNotBeEmpty();
        reviews.Any(r => r.TouristId == -25).ShouldBeTrue();
    }


    [Fact]
    public void Gets_my_review()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 🔹 CLEANUP
        RemoveReviewIfExists(db, -2, -25);

        // 🔹 SEED
        db.TourReviews.Add(new TourReview(
            tourId: -2,
            touristId: -25,
            rating: 4,
            comment: "Nice tour",
            progressPercentage: 60
        ));
        db.SaveChanges();

        // 🔹 ACT
        var review = service.GetMyReview(-2, -25);

        // 🔹 ASSERT
        review.ShouldNotBeNull();
        review.TouristId.ShouldBe(-25);
        review.TourId.ShouldBe(-2);
        review.Rating.ShouldBe(4);
    }
    private static void RemoveReviewIfExists(ToursContext db, long tourId, long touristId)
    {
        var existing = db.TourReviews
            .FirstOrDefault(r => r.TourId == tourId && r.TouristId == touristId);

        if (existing != null)
        {
            db.TourReviews.Remove(existing);
            db.SaveChanges();
        }
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