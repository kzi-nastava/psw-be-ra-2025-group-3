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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Meetup;

[Collection("Sequential")]
public class MeetupQueryTests : BaseStakeholdersIntegrationTest
{
    public MeetupQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Author_retrieves_all_meetups()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<MeetupDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(3); // Imamo 3 meetupa u test podacima
        result.ShouldContain(m => m.Title == "PSW Networking Event");
        result.ShouldContain(m => m.Title == "Travel Enthusiasts Meetup");
    }

    [Fact]
    public void Tourist_retrieves_all_meetups()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<MeetupDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Author_retrieves_meetup_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as MeetupDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe("PSW Networking Event");
        result.CreatorId.ShouldBe(-21);
    }

    [Fact]
    public void GetById_fails_nonexistent_meetup()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.GetById(-999));
    }

    [Fact]
    public void Retrieves_meetups_by_tour_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act
        // U SQL skripti smo povezali Meetup -1 sa Turom -2
        var result = ((ObjectResult)controller.GetByTourId(-2).Result)?.Value as List<MeetupDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();

        // Proveravamo da li svi vraćeni meetupi zaista pripadaju toj turi
        foreach (var meetup in result)
        {
            meetup.TourId.ShouldBe(-2);
        }
    }

    [Fact]
    public void Retrieves_empty_list_for_tour_without_meetups()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        // Act
        // Tura -999 ne postoji ili nema meetupe
        var result = ((ObjectResult)controller.GetByTourId(-999).Result)?.Value as List<MeetupDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private static AuthorMeetupController CreateAuthorController(IServiceScope scope)
    {
        return new AuthorMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }

    private static TouristMeetupController CreateTouristController(IServiceScope scope)
    {
        return new TouristMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}
