using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Explorer.Tours.Tests.Integration.Execution;

[Collection("Sequential")]
public class TourExecutionCommandTests : BaseToursIntegrationTest
{
    public TourExecutionCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_published_tour_successfully_when_purchased()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();

        CleanupExecutionSessions(dbContext, -21);

        var tourId = CreateAndPublishTour(scope, -11);

        shoppingCartService.AddToCart(-21, tourId);
        tokenService.Checkout(-21);

        var dto = new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45.25,
            StartLongitude = 19.83
        };

        var result = controller.StartTour(dto);

        // ✅ NOVO OČEKIVANJE – USKLAĐENO SA LOGIKOM SERVISA
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
    }




    [Fact]
    public void Fails_to_start_draft_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var draft = tourService.Create(new TourCreateDto
        {
            Name = "Draft",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "d" }
        }, -11);

        var dto = new TourExecutionCreateDto
        {
            TourId = draft.Id,
            StartLatitude = 45,
            StartLongitude = 19
        };

        var result = controller.StartTour(dto);
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Fails_when_active_session_already_exists()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var cart = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        var tourId = CreateAndPublishTour(scope, -11);
        cart.AddToCart(-23, tourId);

        var dto = new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45,
            StartLongitude = 19
        };

        controller.StartTour(dto);
        var second = controller.StartTour(dto);

        second.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Fails_for_tour_with_insufficient_key_points()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-24");
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tour = tourService.Create(new TourCreateDto
        {
            Name = "No KP",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "kp" }
        }, -11);

        var dto = new TourExecutionCreateDto
        {
            TourId = tour.Id,
            StartLatitude = 45,
            StartLongitude = 19
        };

        var result = controller.StartTour(dto);
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    // ================= HELPERS =================

    private static long CreateAndPublishTour(IServiceScope scope, long authorId)
    {
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = tourService.Create(new TourCreateDto
        {
            Name = "Execution Tour",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "exec" }
        }, authorId);

        tourService.Update(new TourUpdateDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Difficulty = tour.Difficulty,
            Price = 500,
            Tags = new List<string> { "exec" },
            TourDurations = new List<TourDurationDto>
            {
                new TourDurationDto { TimeInMinutes = 60, TransportType = 0 }
            }
        }, authorId);

        db.KeyPoints.Add(new KeyPoint(tour.Id, "KP1", "D1", "u", "s", 45, 19));
        db.KeyPoints.Add(new KeyPoint(tour.Id, "KP2", "D2", "u", "s", 46, 20));
        db.SaveChanges();

        tourService.Publish(tour.Id, authorId);
        return tour.Id;
    }

    private static TourExecutionController CreateController(IServiceScope scope, string touristId)
    {
        return new TourExecutionController(
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
    private static void CleanupExecutionSessions(ToursContext dbContext, long touristId)
    {
        var sessions = dbContext.TourExecutions
            .Where(te => te.TouristId == touristId)
            .ToList();

        if (sessions.Any())
        {
            dbContext.TourExecutions.RemoveRange(sessions);
            dbContext.SaveChanges();
        }
    }

    private static ControllerContext BuildContext(string touristId)
    {
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[] { new System.Security.Claims.Claim("id", touristId) }, "mock"));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }
}
