using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using Explorer.API.Controllers.Tourist.AppRating;
using Explorer.API.Controllers.Author.AppRating;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.AppRating;

[Collection("Sequential")]
public class AppRatingCommandTests : BaseStakeholdersIntegrationTest
{
    public AppRatingCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    #region Helpers

    private static ClaimsPrincipal BuildUser(long personId)
    {
        var claims = new List<Claim>
        {
            new Claim("personId", personId.ToString())
        };
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

    private static AppRatingAuthorController CreateAuthorController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        var controller = new AppRatingAuthorController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = BuildUser(personId) }
            }
        };
        return controller;
    }

    #endregion

    [Fact]
    public void Tourist_can_update_rating_and_updatedAt_is_set()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -21);

        var getResult = controller.GetMyRating().Result as OkObjectResult;
        var before = getResult!.Value as AppRatingResponseDto;
        var oldCreatedAt = before!.CreatedAt;

        var request = new AppRatingRequestDto
        {
            Rating = 3,
            Comment = "Promenio sam mišljenje."
        };

        // Act
        var updateResult = controller.Update(request).Result as OkObjectResult;

        // Assert
        var updated = updateResult!.Value as AppRatingResponseDto;
        updated!.Rating.ShouldBe(3);
        updated.Comment.ShouldBe("Promenio sam mišljenje.");
        updated.CreatedAt.ShouldBe(oldCreatedAt);
        updated.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Tourist_can_delete_own_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -22);

        var deleteResult = controller.Delete() as OkResult;
        deleteResult.ShouldNotBeNull();

        var getResult = controller.GetMyRating().Result as OkObjectResult;
        getResult!.Value.ShouldBeNull();
    }

    [Fact]
    public void Author_can_create_or_update_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -11);

        var request = new AppRatingRequestDto
        {
            Rating = 4,
            Comment = "Autor: aplikacija je dobra."
        };

        var result = controller.Create(request).Result as OkObjectResult;
        var dto = result!.Value as AppRatingResponseDto;

        dto.ShouldNotBeNull();
        dto!.UserId.ShouldBe(-11);
        dto.Rating.ShouldBe(4);
        dto.Comment.ShouldBe("Autor: aplikacija je dobra.");
    }

    [Fact]
    public void Author_create_rating_with_invalid_rating_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -11);

        var request = new AppRatingRequestDto
        {
            Rating = -9,                      
            Comment = "Ovo ne bi smelo da prođe."
        };

        Should.Throw<ArgumentException>(() => controller.Create(request));
    }

    [Fact]
    public void Update_non_existing_rating_throws_key_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -1);

        var request = new AppRatingRequestDto
        {
            Rating = 3,
            Comment = "Pokušaj update-a bez postojeće ocene."
        };

        Should.Throw<KeyNotFoundException>(() => controller.Update(request));
    }
}

