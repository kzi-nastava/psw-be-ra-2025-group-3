using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Execution;

[Collection("Sequential")]
public class TourExecutionLocationCheckTests : BaseToursIntegrationTest
{
    public TourExecutionLocationCheckTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Completes_key_point_when_near()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo pokreni turu
        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };
        controller.StartTour(startDto);

        // Proveri lokaciju blizu prve KeyPoint (Tour -2 ima KP na 45.254, 19.841)
        var checkDto = new LocationCheckDto
        {
            TourId = -2,
            CurrentLatitude = 45.254000,  // Blizu prve tačke
            CurrentLongitude = 19.841000
        };

        // Act
        var actionResult = controller.CheckLocation(checkDto);

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult.Value as LocationCheckResultDto;

        result.ShouldNotBeNull();
        result.KeyPointCompleted.ShouldBeTrue();
        result.CompletedKeyPointId.ShouldNotBeNull();
        result.TotalCompletedKeyPoints.ShouldBe(1);
        result.LastActivity.ShouldNotBe(default(DateTime));

        // Proveri u bazi
        var storedExecution = dbContext.TourExecutions
            .FirstOrDefault(te => te.TouristId == -21 && te.TourId == -2);
        storedExecution.ShouldNotBeNull();
        storedExecution.CompletedKeyPoints.Count.ShouldBe(1);
    }

    [Fact]
    public void Does_not_complete_when_far()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");

        // Prvo pokreni turu
        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };
        controller.StartTour(startDto);

        // Proveri lokaciju daleko od svih KeyPoints
        var checkDto = new LocationCheckDto
        {
            TourId = -2,
            CurrentLatitude = 45.2000,  // Daleko
            CurrentLongitude = 19.7000
        };

        // Act
        var actionResult = controller.CheckLocation(checkDto);

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult.Value as LocationCheckResultDto;

        result.ShouldNotBeNull();
        result.KeyPointCompleted.ShouldBeFalse();
        result.CompletedKeyPointId.ShouldBeNull();
        result.TotalCompletedKeyPoints.ShouldBe(0);
        result.LastActivity.ShouldNotBe(default(DateTime)); // Ažuriran!
    }

    [Fact]
    public void Updates_last_activity_on_every_check()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo pokreni turu
        var startDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2500,
            StartLongitude = 19.8300
        };
        controller.StartTour(startDto);

        var execution1 = dbContext.TourExecutions
            .FirstOrDefault(te => te.TouristId == -23 && te.TourId == -2);
        var lastActivity1 = execution1.LastActivity;

        Thread.Sleep(100);

        // Prva provera
        var checkDto = new LocationCheckDto
        {
            TourId = -2,
            CurrentLatitude = 45.2000,
            CurrentLongitude = 19.7000
        };
        controller.CheckLocation(checkDto);

        // Refresh
        dbContext.Entry(execution1).Reload();
        var lastActivity2 = execution1.LastActivity;

        Thread.Sleep(100);

        // Druga provera
        controller.CheckLocation(checkDto);

        // Refresh
        dbContext.Entry(execution1).Reload();
        var lastActivity3 = execution1.LastActivity;

        // Assert
        lastActivity2.ShouldBeGreaterThan(lastActivity1);
        lastActivity3.ShouldBeGreaterThan(lastActivity2);
    }

    [Fact]
    public void Fails_without_active_session()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-24");

        var checkDto = new LocationCheckDto
        {
            TourId = -2,
            CurrentLatitude = 45.254000,
            CurrentLongitude = 19.841000
        };

        // Act
        var actionResult = controller.CheckLocation(checkDto);

        // Assert
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
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
