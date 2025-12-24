using Explorer.API.Controllers.Tourist.Preferences;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Tests.Integration.Preferences;

[Collection("Sequential")]
public class PreferenceRecommendationTests : BaseStakeholdersIntegrationTest
{
    public PreferenceRecommendationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_recommended_tours_returns_list()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-101");

        // Act
        var result = ((ObjectResult)controller.GetRecommendedTours().Result)?.Value as List<RecommendedTourDto>;

        // Assert
        result.ShouldNotBeNull();
        // Može biti prazna lista ako nema matching tura, ali ne sme biti null
    }

    [Fact]
    public void Fails_when_tourist_has_no_preferences()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999");

        // Act & Assert
        Should.Throw<Exception>(() => controller.GetRecommendedTours());
    }

    [Fact]
    public void Returned_tours_have_all_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-101");

        // Act
        var result = ((ObjectResult)controller.GetRecommendedTours().Result)?.Value as List<RecommendedTourDto>;

        // Assert
        result.ShouldNotBeNull();

        // Ako ima rezultata, proveri da imaju sve podatke
        if (result.Count > 0)
        {
            var tour = result[0];
            tour.Id.ShouldNotBe(0);
            tour.Name.ShouldNotBeNullOrEmpty();
            tour.Description.ShouldNotBeNullOrEmpty();
            tour.Difficulty.ShouldBeGreaterThanOrEqualTo(0);
            tour.Price.ShouldBeGreaterThanOrEqualTo(0);
            tour.DistanceInKm.ShouldBeGreaterThanOrEqualTo(0);
            tour.Tags.ShouldNotBeNull();
            tour.MatchScore.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void Recommended_tours_are_sorted_by_score()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-101");

        // Act
        var result = ((ObjectResult)controller.GetRecommendedTours().Result)?.Value as List<RecommendedTourDto>;

        // Assert
        result.ShouldNotBeNull();

        // Ako ima više od jednog rezultata, proveri sortiranje
        if (result.Count > 1)
        {
            for (int i = 0; i < result.Count - 1; i++)
            {
                result[i].MatchScore.ShouldBeGreaterThanOrEqualTo(result[i + 1].MatchScore);
            }
        }
    }

    private static PreferenceController CreateController(IServiceScope scope, string touristId)
    {
        return new PreferenceController(scope.ServiceProvider.GetRequiredService<IPreferenceService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
}