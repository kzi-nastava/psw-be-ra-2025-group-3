using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit;

public class TourReviewTests
{
    [Theory]
    [InlineData(1, "Great tour!")]
    [InlineData(3, "Average experience")]
    [InlineData(5, null)]
    public void Creates_valid_tour_review(int rating, string? comment)
    {
        var review = new TourReview(2, 21, rating, comment, 50.0);

        review.ShouldNotBeNull();
        review.TourId.ShouldBe(2);
        review.TouristId.ShouldBe(21);
        review.Rating.ShouldBe(rating);
        review.Comment.ShouldBe(comment);
        review.ProgressPercentage.ShouldBe(50.0);
        review.IsEdited.ShouldBeFalse();
        review.CreatedAt.ShouldNotBe(default(DateTime));
        review.UpdatedAt.ShouldBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Fails_to_create_with_invalid_rating(int rating)
    {
        Should.Throw<ArgumentException>(() =>
            new TourReview(2, 21, rating, "Comment", 50.0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Fails_to_create_with_invalid_progress(double progress)
    {
        Should.Throw<ArgumentException>(() =>
            new TourReview(2, 21, 4, "Comment", progress));
    }

    [Fact]
    public void Updates_review_successfully()
    {
        var review = new TourReview(2, 21, 3, "Original comment", 50.0);
        var originalCreatedAt = review.CreatedAt;

        review.Update(5, "Updated comment");

        review.Rating.ShouldBe(5);
        review.Comment.ShouldBe("Updated comment");
        review.IsEdited.ShouldBeTrue();
        review.UpdatedAt.ShouldNotBeNull();
        review.CreatedAt.ShouldBe(originalCreatedAt);
    }

    [Fact]
    public void Fails_to_update_with_invalid_rating()
    {
        var review = new TourReview(2, 21, 3, "Comment", 50.0);

        Should.Throw<ArgumentException>(() => review.Update(6, "Updated"));
    }
}