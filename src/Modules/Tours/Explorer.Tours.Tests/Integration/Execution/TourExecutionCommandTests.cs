using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Execution;

[Collection("Sequential")]
public class TourExecutionCommandTests : BaseToursIntegrationTest
{
    public TourExecutionCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_published_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        // Act
        var actionResult = controller.StartTour(dto);

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(200);

        var execution = okResult.Value as TourExecutionDto;
        execution.ShouldNotBeNull();
        execution.TouristId.ShouldBe(-21);
        execution.TourId.ShouldBe(-2);
        execution.Status.ShouldBe(0);
        execution.StartLatitude.ShouldBe(45.2500);
        execution.StartLongitude.ShouldBe(19.8300);

        var storedEntity = dbContext.TourExecutions
            .FirstOrDefault(te => te.TouristId == -21 && te.TourId == -2);
        storedEntity.ShouldNotBeNull();
        storedEntity.Status.ShouldBe(TourExecutionStatus.Active);
    }

    [Fact]
    public void Fails_to_start_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var dto = new TourExecutionCreateDto
        {
            TourId = -1,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        // Act
        var actionResult = controller.StartTour(dto);

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();

        var badResult = actionResult.Result as BadRequestObjectResult;
        badResult.ShouldNotBeNull();
        badResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Fails_when_active_session_already_exists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");

        var dto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        // Prvi poziv
        var firstActionResult = controller.StartTour(dto);
        firstActionResult.Result.ShouldBeOfType<OkObjectResult>();

        // Act - Drugi poziv
        var secondActionResult = controller.StartTour(dto);

        // Assert
        secondActionResult.ShouldNotBeNull();
        secondActionResult.Result.ShouldBeOfType<BadRequestObjectResult>();

        var badResult = secondActionResult.Result as BadRequestObjectResult;
        badResult.ShouldNotBeNull();
        badResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Fails_for_tour_with_insufficient_key_points()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var dto = new TourExecutionCreateDto
        {
            TourId = -3,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        // Act
        var actionResult = controller.StartTour(dto);

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();

        var badResult = actionResult.Result as BadRequestObjectResult;
        badResult.ShouldNotBeNull();
        badResult.StatusCode.ShouldBe(400);
    }

    private static TourExecutionController CreateController(IServiceScope scope, string touristId)
    {
        return new TourExecutionController(
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
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