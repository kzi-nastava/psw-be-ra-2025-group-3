using Explorer.API.Controllers.Tourist.Review;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Review;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Review;

[Collection("Sequential")]
public class TourReviewCommandTests : BaseToursIntegrationTest
{
    public TourReviewCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_review_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-28");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var existing = dbContext.TourReviews.Where(r => r.TouristId == -28).ToList(); // ← PROMENJENO
        dbContext.TourReviews.RemoveRange(existing);
        dbContext.SaveChanges();

        var dto = new TourReviewCreateDto
        {
            TourId = -2,
            Rating = 5,
            Comment = "Excellent tour!"
        };

        var actionResult = controller.CreateReview(dto);

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var review = okResult!.Value as TourReviewDto;

        review.ShouldNotBeNull();
        review.TourId.ShouldBe(-2);
        review.TouristId.ShouldBe(-28);
        review.Rating.ShouldBe(5);
        review.Comment.ShouldBe("Excellent tour!");
        review.ProgressPercentage.ShouldBe(40.0);
        review.IsEdited.ShouldBeFalse();
    }

    [Fact]
    public void Fails_to_create_duplicate_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-25");

        var dto = new TourReviewCreateDto
        {
            TourId = -2,
            Rating = 5,
            Comment = "Another review"
        };

        var actionResult = controller.CreateReview(dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Fails_to_create_with_low_progress()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-26");

        var dto = new TourReviewCreateDto
        {
            TourId = -2,
            Rating = 4,
            Comment = "Good tour"
        };

        var actionResult = controller.CreateReview(dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Updates_review_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-25");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var existingReview = dbContext.TourReviews
            .FirstOrDefault(r => r.TouristId == -25 && r.TourId == -2);

        existingReview.ShouldNotBeNull();

        var dto = new TourReviewUpdateDto
        {
            ReviewId = existingReview.Id,
            Rating = 5,
            Comment = "Updated: Even better now!"
        };

        var actionResult = controller.UpdateReview(dto);

        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var review = okResult!.Value as TourReviewDto;

        review.ShouldNotBeNull();
        review.Rating.ShouldBe(5);
        review.Comment.ShouldBe("Updated: Even better now!");
        review.IsEdited.ShouldBeTrue();
        review.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_to_update_other_users_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-28");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var reviewOfOtherUser = dbContext.TourReviews
            .FirstOrDefault(r => r.TouristId == -25 && r.TourId == -2);

        reviewOfOtherUser.ShouldNotBeNull();

        var dto = new TourReviewUpdateDto
        {
            ReviewId = reviewOfOtherUser.Id,
            Rating = 1,
            Comment = "Hacked!"
        };

        var actionResult = controller.UpdateReview(dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static TourReviewController CreateController(IServiceScope scope, string touristId)
    {
        return new TourReviewController(
            scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
            scope.ServiceProvider.GetRequiredService<IPersonRepository>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }

    private static ControllerContext BuildContext(string touristId)
    {
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("id", touristId)
            }, "mock"));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }
}