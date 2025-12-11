using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPreviewQueryTests : BaseToursIntegrationTest
{
    public TourPreviewQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_published_tours_with_details()
    {
        // 1. ARRANGE
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // 2. ACT
        var result = ((OkObjectResult)controller.GetPublishedTours().Result)?.Value as List<TourPreviewDto>;

        // 3. ASSERT
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);

        // Provera da li su podaci popunjeni
        var firstTour = result.First();
        firstTour.Name.ShouldNotBeEmpty();
        firstTour.Description.ShouldNotBeEmpty();
 
    }

    [Fact]
    public void Hides_extra_key_points()
    {
        // 1. ARRANGE
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // 2. ACT
        var result = ((OkObjectResult)controller.GetPublishedTours().Result)?.Value as List<TourPreviewDto>;
        var tour = result.First();

        // 3. ASSERT
        tour.FirstKeyPoint.ShouldNotBeNull();

    }

    [Fact]
    public void Includes_reviews_with_tourist_names()
    {
        // 1. ARRANGE
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // 2. ACT
        var result = ((OkObjectResult)controller.GetPublishedTours().Result)?.Value as List<TourPreviewDto>;

        // 3. ASSERT
        var tourWithReviews = result.FirstOrDefault(t => t.Reviews != null && t.Reviews.Any());

        if (tourWithReviews != null)
        {
            var review = tourWithReviews.Reviews.First();

            review.Comment.ShouldNotBeEmpty();
            review.Rating.ShouldBeGreaterThan(0);


            review.TouristName.ShouldNotBeNullOrEmpty();
        }
    }

    private static TourPreviewController CreateController(IServiceScope scope)
    {
        return new TourPreviewController(
            scope.ServiceProvider.GetRequiredService<ITouristTourService>(),
            scope.ServiceProvider.GetRequiredService<IPersonService>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }
}