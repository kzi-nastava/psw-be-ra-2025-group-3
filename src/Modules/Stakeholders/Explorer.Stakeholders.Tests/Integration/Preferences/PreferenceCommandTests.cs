using Explorer.API.Controllers.Tourist.Preferences;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Preferences;

[Collection("Sequential")]
public class PreferenceCommandTests : BaseStakeholdersIntegrationTest
{
    public PreferenceCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newEntity = new PreferenceCreateDto
        {
            Difficulty = 2,
            WalkingRating = 3,
            BicycleRating = 2,
            CarRating = 1,
            BoatRating = 0,
            Tags = new List<string> { "Mountain", "Hiking" }
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as PreferenceDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(-21);
        result.Difficulty.ShouldBe(2);
        result.WalkingRating.ShouldBe(3);
        result.BicycleRating.ShouldBe(2);
        result.CarRating.ShouldBe(1);
        result.BoatRating.ShouldBe(0);
        result.Tags.Count.ShouldBe(2);

        // Assert - Database
        var storedEntity = dbContext.Preferences.FirstOrDefault(p => p.TouristId == -21);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");
        var invalidEntity = new PreferenceCreateDto
        {
            Difficulty = 1,
            WalkingRating = 5, // Invalid: max is 3
            BicycleRating = 2,
            CarRating = 1,
            BoatRating = 0,
            Tags = new List<string> { "Test" }
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_no_tags()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var invalidEntity = new PreferenceCreateDto
        {
            Difficulty = 1,
            WalkingRating = 3,
            BicycleRating = 2,
            CarRating = 1,
            BoatRating = 0,
            Tags = new List<string>()
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-101");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updatedEntity = new PreferenceUpdateDto
        {
            Id = -1,
            Difficulty = 2,
            WalkingRating = 2,
            BicycleRating = 3,
            CarRating = 2,
            BoatRating = 1,
            Tags = new List<string> { "Adventure", "Sport" }
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as PreferenceDto;

        // Assert
        result.ShouldNotBeNull();
        result.Difficulty.ShouldBe(2);
        result.BicycleRating.ShouldBe(3);
    }

    [Fact]
    public void Update_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999");
        var updatedEntity = new PreferenceUpdateDto
        {
            Id = -1,
            Difficulty = 2,
            WalkingRating = 2,
            BicycleRating = 3,
            CarRating = 2,
            BoatRating = 1,
            Tags = new List<string> { "Test" }
        };

        // Act & Assert
        Should.Throw<Exception>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-103");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = (OkResult)controller.Delete();

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var storedEntity = dbContext.Preferences.FirstOrDefault(p => p.TouristId == -103);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999");

        // Act & Assert
        Should.Throw<Exception>(() => controller.Delete());
    }

    private static PreferenceController CreateController(IServiceScope scope, string touristId)
    {
        return new PreferenceController(scope.ServiceProvider.GetRequiredService<IPreferenceService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
}