using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tourDto = new TourCreateDto
        {
            Name = "New Test Tour",
            Description = "Description for new tour",
            Difficulty = 1,
            Tags = new List<string> { "new", "test" }
        };

        // Act
        try
        {
            var result = service.Create(tourDto, -11);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(tourDto.Name);
            result.Description.ShouldBe(tourDto.Description);
            result.Difficulty.ShouldBe(tourDto.Difficulty);
            result.Status.ShouldBe(0); // Draft
            result.Price.ShouldBe(0);
            result.AuthorId.ShouldBe(-11);
            result.Tags.Count.ShouldBe(2);
        }
        catch (Exception ex)
        {
            // Dodaj breakpoint ovde i vidi exception!
            throw;
        }
    }

    [Fact]
    public void Updates_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        var updateDto = new TourUpdateDto
        {
            Id = -3,
            Name = "Updated Tour Name",
            Description = "Updated description",
            Difficulty = 2,
            Price = 999.99m,
            Tags = new List<string> { "updated" },
            Status = "Draft",
            TourDurations = new List<TourDurationDto>
            {
                new TourDurationDto { TimeInMinutes = 60, TransportType = 0 }
            }
        };

        // Act
        var result = service.Update(updateDto, -11); 

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Tour Name");
        result.Description.ShouldBe("Updated description");
        result.Difficulty.ShouldBe(2);
        result.Price.ShouldBe(999.99m);
        result.UpdatedAt.ShouldNotBeNull();
        result.TourDurations.Count.ShouldBe(1);
    }

    [Fact]
    public void Deletes_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tempTour = service.Create(new TourCreateDto { Name = "To Delete", Description = "...", Difficulty = 0, Tags = new List<string> { "t" } }, -11);

        service.Delete(tempTour.Id, -11); 

        // Verify by trying to get deleted tour
        Should.Throw<Exception>(() => service.GetById(tempTour.Id));
    }

    [Fact]
    public void Fails_to_delete_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            service.Delete(-2, -11) 
        ).Message.ShouldContain("Draft");
    }

    [Fact]
    public void Publishes_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourDto = new TourCreateDto
        {
            Name = "Publish Test",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "tag" }
        };
        var createdTour = service.Create(tourDto, -11);

        var tourEntity = dbContext.Tours.Find(createdTour.Id);
        tourEntity.UpdateTourDurations(new List<TourDuration> { new TourDuration(60, TransportType.Walking) });

        dbContext.KeyPoints.Add(new KeyPoint(createdTour.Id, "KP1", "D1", "u", "s", 45.0, 19.0));
        dbContext.KeyPoints.Add(new KeyPoint(createdTour.Id, "KP2", "D2", "u", "s", 45.1, 19.1));
        dbContext.SaveChanges();

        // Act
        var result = service.Publish(createdTour.Id, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Published
        result.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_to_update_other_authors_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        var updateDto = new TourUpdateDto
        {
            Id = -1,
            Name = "Hacked Tour",
            Description = "Trying to hack",
            Difficulty = 0,
            Price = 0,
            Tags = new List<string>(),
            TourDurations = new List<TourDurationDto>()
        };

        // Act & Assert
        Should.Throw<Exception>(() =>
            service.Update(updateDto, -999)
        );
    }
}