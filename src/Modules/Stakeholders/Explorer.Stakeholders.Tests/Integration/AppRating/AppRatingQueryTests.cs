using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using Explorer.API.Controllers.Tourist.AppRating;
using Explorer.API.Controllers.Administrator.AppRating;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.AppRating;

[Collection("Sequential")]
public class AppRatingQueryTests : BaseStakeholdersIntegrationTest
{
    public AppRatingQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    #region Helpers

    private static ClaimsPrincipal BuildUser(long id)
    {
        var claims = new List<Claim> { new Claim("personId", id.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private static AppRatingTouristController CreateTouristController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        var controller = new AppRatingTouristController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = BuildUser(personId) }
            }
        };
        return controller;
    }

    private static AppRatingAdministrationController CreateAdminController(IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        return new AppRatingAdministrationController(service);
    }

    #endregion

    [Fact]
    public void Tourist_can_get_own_seed_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -21);

        var result = controller.GetMyRating().Result as OkObjectResult;

        var dto = result!.Value as AppRatingResponseDto;
        dto.ShouldNotBeNull();
        dto!.UserId.ShouldBe(-21);
        dto.Rating.ShouldBe(5); 
    }

    [Fact]
    public void Tourist_without_rating_gets_null()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -1);

        var result = controller.GetMyRating().Result as OkObjectResult;
        result.ShouldNotBeNull();
        result!.Value.ShouldBeNull();
    }

    [Fact]
    public void Admin_can_get_all_ratings()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        var result = controller.GetAll().Result as OkObjectResult;

        var list = result!.Value as List<AppRatingResponseDto>;
        list.ShouldNotBeNull();
        list!.Count.ShouldBeGreaterThan(0);

        list.Any(r => r.UserId == -21).ShouldBeTrue();
    }
}
