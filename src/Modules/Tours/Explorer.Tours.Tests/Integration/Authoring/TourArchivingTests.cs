using Explorer.API.Controllers.Author.Authoring;
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
public class TourArchivingTests : BaseToursIntegrationTest
{
    public TourArchivingTests(ToursTestFactory factory) : base(factory) { }

    private TourDto CreatePublishedTour(ITourService service, ToursContext dbContext)
    {
        var tourDto = new TourCreateDto
        {
            Name = "Tour For Archiving",
            Description = "Description",
            Difficulty = 0,
            Tags = new List<string> { "archiving-test" }
        };
        var tour = service.Create(tourDto, -11);

        var entity = dbContext.Tours.Find(tour.Id);

        dbContext.KeyPoints.Add(new KeyPoint(tour.Id, "KP1", "Desc", "img", "sec", 45.0, 19.0));
        dbContext.KeyPoints.Add(new KeyPoint(tour.Id, "KP2", "Desc", "img", "sec", 45.1, 19.1));

        entity.UpdateTourDurations(new List<TourDuration> { new TourDuration(60, TransportType.Walking) });

        dbContext.SaveChanges();

        return service.Publish(tour.Id, -11);
    }

    [Fact]
    public void Archives_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var publishedTour = CreatePublishedTour(service, dbContext);

        // Act
        var result = service.Archive(publishedTour.Id, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(2); // 2 = Archived (proveri svoj Enum, obično je Draft=0, Published=1, Archived=2)
        result.ArchivedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Reactivates_archived_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo kreiramo i arhiviramo turu
        var tour = CreatePublishedTour(service, dbContext);
        service.Archive(tour.Id, -11);

        // Act
        var result = service.Reactivate(tour.Id, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Vraća se u Published
        result.ArchivedAt.ShouldBeNull(); // Više nije arhivirana
    }

    [Fact]
    public void Fails_to_archive_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Kreiramo samo Draft turu (nismo je objavili)
        var tourDto = new TourCreateDto
        {
            Name = "Draft Tour",
            Description = "Desc",
            Difficulty = 0,
            Tags = new List<string> { "tag" }
        };
        var draftTour = service.Create(tourDto, -11);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Archive(draftTour.Id, -11)
        );
        exception.Message.ShouldContain("Only published tours can be archived");
    }

    [Fact]
    public void Fails_to_reactivate_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var publishedTour = CreatePublishedTour(service, dbContext);

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Reactivate(publishedTour.Id, -11)
        );
        exception.Message.ShouldContain("Only archived tours can be reactivated");
    }

    [Fact]
    public void Fails_to_update_archived_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Pripremi Arhiviranu turu
        var tour = CreatePublishedTour(service, dbContext);
        service.Archive(tour.Id, -11);

        // Pokušaj izmene (Update)
        var updateDto = new TourUpdateDto
        {
            Id = tour.Id,
            Name = "Changed Name",
            Description = "Changed Desc",
            Difficulty = 0,
            Tags = new List<string> { "tag" },
            Price = 100,
            TourDurations = new List<TourDurationDto>()
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Update(updateDto, -11)
        );
        exception.Message.ShouldContain("Cannot modify an archived tour");
    }
}