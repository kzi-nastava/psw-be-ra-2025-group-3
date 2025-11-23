using Explorer.API.Controllers.Administrator.AppRating;
using Explorer.API.Controllers.Author.AppRating;
using Explorer.API.Controllers.Tourist.AppRating;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

    private static ControllerContext BuildContext(long personId)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildUser(personId)
            }
        };
    }

    private static AppRatingAuthorController CreateAuthorController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        return new AppRatingAuthorController(service)
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static AppRatingTouristController CreateTouristController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        return new AppRatingTouristController(service)
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static AppRatingAdministrationController CreateAdministrationController(IServiceScope scope)
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
    public void Author_can_get_own_seed_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -11);

        var result = controller.GetMyRating().Result as OkObjectResult;

        var dto = result!.Value as AppRatingResponseDto;
        dto.ShouldNotBeNull();
        dto!.UserId.ShouldBe(-11);
        dto.Rating.ShouldBe(5);
    }

    [Fact]
    public void Author_without_rating_gets_null()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -1);

        var result = controller.GetMyRating().Result as OkObjectResult;
        result.ShouldNotBeNull();
        result!.Value.ShouldBeNull();
    }

    [Fact]
    public void Admin_can_get_all_ratings()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdministrationController(scope);

        var result = controller.GetAll(page: 1, pageSize: 6).Result as OkObjectResult;
        var paged = result!.Value as PagedResult<AppRatingResponseDto>;

        paged.ShouldNotBeNull();
        paged!.Results.Count.ShouldBeGreaterThan(0);
        paged.Results.Any(r => r.UserId == -21).ShouldBeTrue();
        paged.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Administration_pagination_respects_page_size()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdministrationController(scope);

        const int pageSize = 2;

        var result = controller.GetAll(page: 1, pageSize: pageSize).Result as OkObjectResult;
        var paged = result!.Value as PagedResult<AppRatingResponseDto>;

        paged.ShouldNotBeNull();
        paged!.Results.Count.ShouldBeLessThanOrEqualTo(pageSize);
        paged.TotalCount.ShouldBeGreaterThanOrEqualTo(paged.Results.Count);
    }
}
