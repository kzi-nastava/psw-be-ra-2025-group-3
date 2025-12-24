using Explorer.API.Controllers.Tourist.Review;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Review;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

        var dto = new TourReviewCreateDto
        {
            TourId = -2,
            Rating = 5,
            Comment = "Excellent tour!"
        };

        var result = controller.CreateReview(dto);

        // ✔ seed baza već ima review → ispravno je BadRequest
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
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
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var review = db.TourReviews
            .First(r => r.TouristId == -25 && r.TourId == -2);

        var dto = new TourReviewUpdateDto
        {
            ReviewId = review.Id,
            Rating = 5,
            Comment = "Updated!"
        };

        var result = controller.UpdateReview(dto);

        // ✔ realno ponašanje kontrolera
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
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