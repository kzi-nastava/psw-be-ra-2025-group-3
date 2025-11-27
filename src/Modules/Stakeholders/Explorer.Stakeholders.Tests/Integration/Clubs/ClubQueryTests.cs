using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubQueryTests : BaseStakeholdersIntegrationTest
{
    public ClubQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_club_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act
        var result = service.Get(-1);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe("Hiking Club");
        result.OwnerId.ShouldBe(-21);
        result.FeaturedImageId.ShouldNotBe(0);
        result.FeaturedImage.ShouldNotBeNull();
    }

    [Fact]
    public void Gets_all_clubs_paged()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act
        var result = service.GetPaged(1, 2);

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Gets_clubs_by_owner_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act
        var result = service.GetUserClubs(-21, 1, 10);

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Results.All(c => c.OwnerId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Fails_to_get_nonexistent_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => service.Get(-999));
    }

    [Fact]
    public void Pagination_works_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClubService>();

        // Act
        var page1 = service.GetPaged(1, 2);
        var page2 = service.GetPaged(2, 2);

        // Assert
        page1.Results.Count.ShouldBe(2);
        page2.Results.Count.ShouldBeGreaterThan(0);
        page1.Results[0].Id.ShouldNotBe(page2.Results[0].Id);
    }
}
