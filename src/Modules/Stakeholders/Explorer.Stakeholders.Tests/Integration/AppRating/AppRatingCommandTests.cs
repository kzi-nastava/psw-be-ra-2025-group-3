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

    private static AppRatingTouristController CreateTouristController(IServiceScope scope, long personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IAppRatingService>();
        return new AppRatingTouristController(service)
        {
            ControllerContext = BuildContext(personId)
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
        updated.ShouldNotBeNull();
        updated!.UserId.ShouldBe(-21);
        updated.Rating.ShouldBe(3);
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
    public void Tourist_can_create_rating_after_deletion()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -23);

        var deleteResult = controller.Delete() as OkResult;
        deleteResult.ShouldNotBeNull();

        var createRequest = new AppRatingRequestDto
        {
            Rating = 5,
            Comment = "Nova ocena nakon brisanja."
        };

        var createResult = controller.Create(createRequest).Result as OkObjectResult;
        var dto = createResult!.Value as AppRatingResponseDto;

        dto.ShouldNotBeNull();
        dto!.UserId.ShouldBe(-23);
        dto.Rating.ShouldBe(5);
        dto.Comment.ShouldBe("Nova ocena nakon brisanja.");
        dto.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public void Tourist_create_rating_with_invalid_rating_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -21);

        var request = new AppRatingRequestDto
        {
            Rating = -9,
            Comment = "Ovo ne bi smelo da prođe."
        };

        Should.Throw<ArgumentException>(() => controller.Create(request));
    }

    [Fact]
    public void Tourist_update_non_existing_rating_throws_key_not_found()
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

    [Fact]
    public void Tourist_delete_non_existing_rating_throws_key_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, -999);
        Should.Throw<KeyNotFoundException>(() => controller.Delete());
    }

    [Fact]
    public void Author_can_update_existing_rating_and_updatedAt_is_set()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -12);

        var getResult = controller.GetMyRating().Result as OkObjectResult;
        var before = getResult!.Value as AppRatingResponseDto;
        var oldCreatedAt = before!.CreatedAt;

        var request = new AppRatingRequestDto
        {
            Rating = 4,
            Comment = "Autor: promenio sam mišljenje."
        };

        var updateResult = controller.Update(request).Result as OkObjectResult;
        var updated = updateResult!.Value as AppRatingResponseDto;

        updated.ShouldNotBeNull();
        updated!.UserId.ShouldBe(-12);
        updated.Rating.ShouldBe(4);
        updated.Comment.ShouldBe("Autor: promenio sam mišljenje.");
        updated.CreatedAt.ShouldBe(oldCreatedAt);
        updated.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Author_can_delete_own_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -11);

        var deleteResult = controller.Delete() as OkResult;
        deleteResult.ShouldNotBeNull();

        var getResult = controller.GetMyRating().Result as OkObjectResult;
        getResult.ShouldNotBeNull();
        getResult!.Value.ShouldBeNull();
    }


    [Fact]
    public void Author_can_create_rating_after_deletion()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -13);

        controller.Delete();

        var request = new AppRatingRequestDto
        {
            Rating = 5,
            Comment = "Autor: nova ocena posle brisanja."
        };

        var result = controller.Create(request).Result as OkObjectResult;
        var dto = result!.Value as AppRatingResponseDto;

        dto.ShouldNotBeNull();
        dto!.UserId.ShouldBe(-13);
        dto.Rating.ShouldBe(5);
        dto.Comment.ShouldBe("Autor: nova ocena posle brisanja.");
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

    [Fact]
    public void Author_delete_non_existing_rating_throws_key_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, -999);

        Should.Throw<KeyNotFoundException>(() => controller.Delete());
    }
}

