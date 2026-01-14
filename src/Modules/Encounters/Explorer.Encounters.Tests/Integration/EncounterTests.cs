using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
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
    public void Create_Success_HiddenLocation()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "Hidden Treasure",
            Description = "Find the hidden treasure using the image hint",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 100,
            Type = "HiddenLocation",
            Status = "Draft",
            ImageUrl = "https://picsum.photos/id/50/4608/3072"
        };

        // Act
        var result = service.Create(encounterDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe("Hidden Treasure");
        result.Type.ShouldBe("HiddenLocation");
        result.ImageUrl.ShouldBe("https://picsum.photos/id/50/4608/3072");
    }

    [Fact]
    public void Create_Success_MiscEncounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "Push-ups Challenge",
            Description = "Complete 20 push-ups",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 50,
            Type = "Misc",
            Status = "Draft",
            ActionDescription = "Do 20 push-ups at this location"
        };

        // Act
        var result = service.Create(encounterDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe("Push-ups Challenge");
        result.Type.ShouldBe("Misc");
        result.ActionDescription.ShouldBe("Do 20 push-ups at this location");
    }

    [Fact]
    public void Create_Success_SocialEncounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

        var encounterDto = new EncounterDto
        {
            Name = "Group Meetup",
            Description = "Meet with other tourists",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 80,
            Type = "Social",
            Status = "Draft",
            RequiredPeopleCount = 5,
            RangeInMeters = 20.0
        };

        // Act
        var result = service.Create(encounterDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe("Group Meetup");
        result.Type.ShouldBe("Social");
        result.RequiredPeopleCount.ShouldBe(5);
        result.RangeInMeters.ShouldBe(20.0);
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
            Status = "Draft",
            RequiredPeopleCount = 3,
            RangeInMeters = 15.0
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
            Type = "HiddenLocation",
            Status = "Draft",
            ImageUrl = "https://picsum.photos/id/60/1920/1200"
        };

        var activeEncounter = new EncounterDto
        {
            Name = "Active Encounter",
            Description = "This is active",
            Latitude = 45.2671,
            Longitude = 19.8335,
            XP = 100,
            Type = "Social",
            Status = "Active",
            RequiredPeopleCount = 4,
            RangeInMeters = 20.0
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
            Status = "Draft",
            ActionDescription = "Simple action"
        };
        var created = service.Create(encounterDto);

        // Act
        service.Delete(created.Id);

        // Assert
        Should.Throw<KeyNotFoundException>(() => service.Get(created.Id));
    }
}