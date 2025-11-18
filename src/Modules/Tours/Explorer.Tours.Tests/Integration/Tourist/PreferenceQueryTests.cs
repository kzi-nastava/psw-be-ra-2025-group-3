using Explorer.API.Controllers.Tourist.Preferences;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class PreferenceQueryTests : BaseToursIntegrationTest
{
    public PreferenceQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_preference_by_tourist_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-101");

        // Act
        var result = ((ObjectResult)controller.GetMyPreferences().Result)?.Value as PreferenceDto;

        // Assert
        result.ShouldNotBeNull();
        result.TouristId.ShouldBe(-101);
        result.Difficulty.ShouldBe(1);
        result.WalkingRating.ShouldBe(3);
        result.Tags.Count.ShouldBe(2);
    }

    [Fact]
    public void Get_fails_when_preference_not_found()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999");

        // Act & Assert
        Should.Throw<Exception>(() => controller.GetMyPreferences());
    }

    private static PreferenceController CreateController(IServiceScope scope, string touristId)
    {
        return new PreferenceController(scope.ServiceProvider.GetRequiredService<IPreferenceService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
}