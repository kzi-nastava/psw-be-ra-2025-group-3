using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Meetup;

[Collection("Sequential")]
public class MeetupCommandTests : BaseStakeholdersIntegrationTest
{
    public MeetupCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Author_creates_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newMeetup = new MeetupCreateDto
        {
            Title = "New Author Meetup",
            Description = "Meetup created by author for testing",
            DateTime = DateTime.UtcNow.AddDays(10),
            Latitude = 45.5m,
            Longitude = 20.0m
        };

        // Act
        var result = ((ObjectResult)controller.Create(newMeetup).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(newMeetup.Title);
        result.CreatorId.ShouldBe(-11); // Author ID

        // Assert - Database
        var storedMeetup = dbContext.Meetups.FirstOrDefault(m => m.Title == newMeetup.Title);
        storedMeetup.ShouldNotBeNull();
        storedMeetup.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Tourist_creates_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newMeetup = new MeetupCreateDto
        {
            Title = "New Tourist Meetup",
            Description = "Meetup created by tourist for testing",
            DateTime = DateTime.UtcNow.AddDays(15),
            Latitude = 44.5m,
            Longitude = 21.0m
        };

        // Act
        var result = ((ObjectResult)controller.Create(newMeetup).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(newMeetup.Title);
        result.CreatorId.ShouldBe(-21); // Tourist ID

        // Assert - Database
        var storedMeetup = dbContext.Meetups.FirstOrDefault(m => m.Title == newMeetup.Title);
        storedMeetup.ShouldNotBeNull();
    }

    [Fact]
    public void Create_fails_invalid_datetime_in_past()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);
        var invalidMeetup = new MeetupCreateDto
        {
            Title = "Invalid Meetup",
            Description = "This should fail",
            DateTime = DateTime.UtcNow.AddDays(-5), // Prošlost!
            Latitude = 45.0m,
            Longitude = 20.0m
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidMeetup));
    }

    [Fact]
    public void Author_updates_own_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updateDto = new MeetupUpdateDto
        {
            Title = "Updated Photography & Travel",
            Description = "Updated description for the event",
            DateTime = new DateTime(2026, 1, 15, 18, 0, 0, DateTimeKind.Utc), 
            Latitude = 45.3m,
            Longitude = 19.9m
        };

        // Act
        var result = ((ObjectResult)controller.Update(-3, updateDto).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-3);
        result.Title.ShouldBe(updateDto.Title);
        result.Description.ShouldBe(updateDto.Description);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedMeetup = dbContext.Meetups.FirstOrDefault(m => m.Id == -3);
        storedMeetup.ShouldNotBeNull();
        storedMeetup.Title.ShouldBe(updateDto.Title);
    }

    [Fact]
    public void Tourist_cannot_update_other_users_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope); // Tourist 1 (-21)
        var updateDto = new MeetupUpdateDto
        {
            Title = "Hacked Title",
            Description = "Trying to update someone else's meetup",
            DateTime = DateTime.UtcNow.AddDays(20),
            Latitude = 45.0m,
            Longitude = 20.0m
        };

        // Act & Assert
        // Tourist 1 pokušava da izmeni meetup Tourist 2 (-2, kreiran od -22)
        Should.Throw<ForbiddenException>(() => controller.Update(-2, updateDto));
    }

    [Fact]
    public void Author_deletes_own_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = (OkResult)controller.Delete(-4); 

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedMeetup = dbContext.Meetups.FirstOrDefault(m => m.Id == -4);
        storedMeetup.ShouldBeNull();
    }

    [Fact]
    public void Tourist_cannot_delete_other_users_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope); // Tourist 1 (-21)

        // Act & Assert
        // Tourist 1 pokušava da obriše meetup Tourist 2 (-2, kreiran od -22)
        Should.Throw<ForbiddenException>(() => controller.Delete(-2));
    }

    [Fact]
    public void Delete_fails_nonexistent_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-999));
    }

    [Fact]
    public void Creates_meetup_with_tour_link()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        // POPRAVKA 1: Koristimo postojeću helper metodu 'CreateAuthorController'
        var controller = CreateAuthorController(scope);

        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newMeetup = new MeetupCreateDto
        {
            Title = "Tura Povezivanje Test",
            Description = "Opis",
            DateTime = DateTime.UtcNow.AddDays(10),

            // POPRAVKA 2: Dodato 'm' na kraju brojeva jer su tipa decimal
            Latitude = 45.2m,
            Longitude = 19.8m,

            TourId = -2 // Vežemo za turu koja postoji u seed-u
        };

        // Act
        var result = ((ObjectResult)controller.Create(newMeetup).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(newMeetup.Title);
        result.TourId.ShouldBe(newMeetup.TourId);

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(i => i.Title == newMeetup.Title);
        storedEntity.ShouldNotBeNull();
        storedEntity.TourId.ShouldBe(newMeetup.TourId);
    }

    private static AuthorMeetupController CreateAuthorController(IServiceScope scope)
    {
        return new AuthorMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-11") // Author 1 personId
        };
    }

    private static TouristMeetupController CreateTouristController(IServiceScope scope)
    {
        return new TouristMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-21") // Tourist 1 personId
        };
    }
}
