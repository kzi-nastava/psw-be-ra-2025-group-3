using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
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
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var shoppingCart = scope.ServiceProvider.GetRequiredService<IShoppingCartService>(); // 👈

        CleanupActiveSessions(dbContext, -21);

        var tourId = CreateAndPublishTour(scope);

        shoppingCart.AddToCart(-21, tourId); // ✅ KLJUČNO

        controller.StartTour(new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45.25,
            StartLongitude = 19.83
        });

        var actionResult = controller.CheckLocation(new LocationCheckDto
        {
            TourId = tourId,
            CurrentLatitude = 45,
            CurrentLongitude = 19
        });

        actionResult.Result.ShouldBeOfType<OkObjectResult>();

        var result = (actionResult.Result as OkObjectResult)!.Value as LocationCheckResultDto;
        result.ShouldNotBeNull();
        result.KeyPointCompleted.ShouldBeTrue();
        result.TotalCompletedKeyPoints.ShouldBe(1);
    }

    [Fact]
    public void Does_not_complete_when_far()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var shoppingCart = scope.ServiceProvider.GetRequiredService<IShoppingCartService>(); // 👈

        CleanupActiveSessions(dbContext, -22);

        var tourId = CreateAndPublishTour(scope);
        shoppingCart.AddToCart(-22, tourId); // ✅

        controller.StartTour(new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45.25,
            StartLongitude = 19.83
        });

        var actionResult = controller.CheckLocation(new LocationCheckDto
        {
            TourId = tourId,
            CurrentLatitude = 0,
            CurrentLongitude = 0
        });

        var result = (actionResult.Result as OkObjectResult)!.Value as LocationCheckResultDto;
        result.ShouldNotBeNull();
        result.KeyPointCompleted.ShouldBeFalse();
        result.TotalCompletedKeyPoints.ShouldBe(0);
    }

    [Fact]
    public void Updates_last_activity_on_every_check()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var shoppingCart = scope.ServiceProvider.GetRequiredService<IShoppingCartService>(); // 👈

        CleanupActiveSessions(dbContext, -23);

        var tourId = CreateAndPublishTour(scope);
        shoppingCart.AddToCart(-23, tourId); // ✅

        controller.StartTour(new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45.25,
            StartLongitude = 19.83
        });

        var execution = dbContext.TourExecutions.First(te =>
            te.TouristId == -23 && te.TourId == tourId && te.Status == TourExecutionStatus.Active);

        var first = execution.LastActivity;

        Thread.Sleep(50);
        controller.CheckLocation(new LocationCheckDto { TourId = tourId, CurrentLatitude = 0, CurrentLongitude = 0 });
        dbContext.Entry(execution).Reload();
        var second = execution.LastActivity;

        Thread.Sleep(50);
        controller.CheckLocation(new LocationCheckDto { TourId = tourId, CurrentLatitude = 0, CurrentLongitude = 0 });
        dbContext.Entry(execution).Reload();
        var third = execution.LastActivity;

        second.ShouldBeGreaterThan(first);
        third.ShouldBeGreaterThan(second);
    }

    // ================= HELPERS =================

    private static long CreateAndPublishTour(IServiceScope scope)
    {
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = tourService.Create(new TourCreateDto
        {
            Name = "Location Test Tour",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "loc" }
        }, -11);

        tourService.Update(new TourUpdateDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Difficulty = tour.Difficulty,
            Price = 100,
            Tags = new List<string> { "loc" },
            TourDurations = new List<TourDurationDto>
            {
                new TourDurationDto { TimeInMinutes = 60, TransportType = 0 }
            }
        }, -11);

        db.KeyPoints.Add(new KeyPoint(tour.Id, "KP1", "D1", "u", "s", 45, 19));
        db.KeyPoints.Add(new KeyPoint(tour.Id, "KP2", "D2", "u", "s", 46, 20));
        db.SaveChanges();

        tourService.Publish(tour.Id, -11);
        return tour.Id;
    }

    private static void CleanupActiveSessions(ToursContext dbContext, long touristId)
    {
        var activeSessions = dbContext.TourExecutions
            .Where(te => te.TouristId == touristId && te.Status == TourExecutionStatus.Active)
            .ToList();

        if (activeSessions.Any())
        {
            dbContext.TourExecutions.RemoveRange(activeSessions);
            dbContext.SaveChanges();
        }
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
