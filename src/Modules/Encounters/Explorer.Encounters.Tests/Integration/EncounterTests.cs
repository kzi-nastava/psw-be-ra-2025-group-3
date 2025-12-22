using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class EncounterTests : BaseEncountersIntegrationTest
{
    public EncounterTests(EncountersTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public void Create_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "Hidden Treasure",
            Description = "Find the hidden treasure at the old castle",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 100,
            Type = "Location",
            Status = "Draft"
        };

        // Act
        var result = service.Create(encounterDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe("Hidden Treasure");
        result.Status.ShouldBe("Draft");
    }

    [Fact]
    public void Update_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "Old Challenge",
            Description = "Original description",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 50,
            Type = "Social",
            Status = "Draft"
        };

        var created = service.Create(encounterDto);

        // Act
        created.Name = "Updated Challenge";
        created.XP = 150;
        var result = service.Update(created);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Challenge");
        result.XP.ShouldBe(150);
    }

    [Fact]
    public void GetActiveEncounters_ReturnsOnlyActive()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var draftEncounter = new EncounterDto
        {
            Name = "Draft Encounter",
            Description = "This is draft",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 50,
            Type = "Location",
            Status = "Draft"
        };

        var activeEncounter = new EncounterDto
        {
            Name = "Active Encounter",
            Description = "This is active",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 100,
            Type = "Social",
            Status = "Active"
        };

        service.Create(draftEncounter);
        service.Create(activeEncounter);

        // Act
        var result = service.GetActiveEncounters();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.ShouldAllBe(e => e.Status == "Active");
    }

    [Fact]
    public void Delete_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "To Be Deleted",
            Description = "This will be deleted",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 50,
            Type = "Misc",
            Status = "Draft"
        };

        var created = service.Create(encounterDto);

        // Act
        service.Delete(created.Id);

        // Assert
        Should.Throw<KeyNotFoundException>(() => service.Get(created.Id));
    }
}