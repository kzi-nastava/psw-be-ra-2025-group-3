using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using Explorer.Payments.API.Public.Shopping;

namespace Explorer.Tours.Tests.Integration.Execution;

[Collection("Sequential")]
public class TourExecutionCommandTests : BaseToursIntegrationTest
{
    public TourExecutionCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_published_tour_successfully_when_purchased()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Kupi turu
        shoppingCartService.AddToCart(-21, -2);

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
        var execution = okResult!.Value as TourExecutionDto;
        execution.ShouldNotBeNull();
        execution.TouristId.ShouldBe(-21);
        execution.TourId.ShouldBe(-2);
        execution.Status.ShouldBe(0);
    }


    [Fact]
    public void Completes_active_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. OČISTI PRETHODNE SESIJE ZA OVOG TURISTA
        var existingSessions = dbContext.TourExecutions
            .Where(te => te.TouristId == -23)
            .ToList();
        dbContext.TourExecutions.RemoveRange(existingSessions);
        dbContext.SaveChanges();

        // 2. KUPI TURU
        shoppingCartService.AddToCart(-23, -2);

        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        // 3. POKRENI TURU
        var startResult = controller.StartTour(startDto);
        startResult.Result.ShouldBeOfType<OkObjectResult>();

        // 4. UČITAJ EXECUTION IZ BAZE
        var execution = dbContext.TourExecutions
            .FirstOrDefault(te => te.TouristId == -23 && te.TourId == -2 && te.Status == TourExecutionStatus.Active);

        execution.ShouldNotBeNull();

        // 5. UZMI SVE KEYPOINTS ZA OVU TURU
        var keyPoints = dbContext.KeyPoints
            .Where(kp => kp.TourId == -2)
            .OrderBy(kp => kp.Id)
            .ToList();

        keyPoints.Count.ShouldBeGreaterThan(0);

        // 6. RUČNO KOMPLЕTIRAJ SVE KEYPOINTS (direktno pozivom agregat metode)
        foreach (var keyPoint in keyPoints)
        {
            // Pozovi CheckLocationProgress sa TAČNIM koordinatama KeyPointa
            // Ovo će automatski dodati KeyPoint u CompletedKeyPoints listu
            execution.CheckLocationProgress(
                keyPoint.Latitude,
                keyPoint.Longitude,
                keyPoints
            );
        }

        // 7. SAČUVAJ PROMENE U BAZI
        dbContext.SaveChanges();

        // 8. OSVJEŽI CONTEXT DA BISMO VIDELI NAJNOVIJE PROMENE
        dbContext.ChangeTracker.Clear();

        // 9. PONOVO UČITAJ EXECUTION DA PROVERIŠ DA SU KEYPOINTS KOMPLETTIRANI
        execution = dbContext.TourExecutions
            .FirstOrDefault(te => te.Id == execution.Id);

        execution.ShouldNotBeNull();
        execution.CompletedKeyPoints.Count.ShouldBe(keyPoints.Count); // Proveri da su SVI komplettirani

        // Act
        var actionResult = controller.CompleteTour();

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var completedExecution = okResult!.Value as TourExecutionDto;
        completedExecution.ShouldNotBeNull();
        completedExecution.Status.ShouldBe(1); // Completed
        completedExecution.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public void Abandons_active_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-24");
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        shoppingCartService.AddToCart(-24, -2);

        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };
        controller.StartTour(startDto);

        // Act
        var actionResult = controller.AbandonTour();

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var execution = okResult!.Value as TourExecutionDto;
        execution.ShouldNotBeNull();
        execution.Status.ShouldBe(2);
        execution.AbandonTime.ShouldNotBeNull();
    }

    [Fact]
    public void Gets_active_tour_with_next_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-25");
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        shoppingCartService.AddToCart(-25, -2);

        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };
        controller.StartTour(startDto);

        // Act
        var response = controller.GetActiveWithNextKeyPoint();

        // Assert
        response.ShouldNotBeNull();
        response.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = response.Result as OkObjectResult;
        var result = okResult!.Value as TourExecutionWithNextKeyPointDto;

        result.ShouldNotBeNull();
        result.NextKeyPoint.ShouldNotBeNull();
        result.DistanceToNextKeyPoint.ShouldNotBeNull();
        result.DistanceToNextKeyPoint.Value.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Fails_to_start_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-26");

        var dto = new TourExecutionCreateDto
        {
            TourId = -1,  // Draft tour (status = 0)
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

        var responseValue = badResult.Value;
        responseValue.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_when_active_session_already_exists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-27");
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        shoppingCartService.AddToCart(-27, -2);

        var dto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };

        controller.StartTour(dto);

        // Act
        var secondActionResult = controller.StartTour(dto);

        // Assert
        secondActionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Fails_for_tour_with_insufficient_key_points()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Očisti prethodne sesije
        var existingSessions = dbContext.TourExecutions
            .Where(te => te.TouristId == -21)
            .ToList();
        dbContext.TourExecutions.RemoveRange(existingSessions);
        dbContext.SaveChanges();

        var dto = new TourExecutionCreateDto
        {
            TourId = -3, // Tour sa nedovoljno KeyPoints (0)
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