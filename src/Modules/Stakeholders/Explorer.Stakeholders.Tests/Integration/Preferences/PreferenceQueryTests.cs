using Explorer.API.Controllers.Tourist.Preferences;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Preferences;

[Collection("Sequential")]
public class PreferenceQueryTests : BaseStakeholdersIntegrationTest
{
    public PreferenceQueryTests(StakeholdersTestFactory factory) : base(factory) { }

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

    private static PreferenceController CreateController(IServiceScope scope, string touristId)
    {
        return new PreferenceController(scope.ServiceProvider.GetRequiredService<IPreferenceService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
}