using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class KeyPointQueryTests : BaseToursIntegrationTest
{
    public KeyPointQueryTests(ToursTestFactory factory) : base(factory) { }

    private long CreateTestTour(ITourService tourService)
    {
        var tourDto = new TourCreateDto
        {
            Name = "KeyPoint Query Tour",
            Description = "Tour for key point query tests",
            Difficulty = 1,
            Tags = new List<string> { "kp-query" }
        };

        var created = tourService.Create(tourDto, -11);
        created.Id.ShouldNotBe(0);
        return created.Id;
    }

    [Fact]
    public void Gets_key_point_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var tourId = CreateTestTour(tourService);

        var created = keyPointService.Create(new KeyPointDto
        {
            TourId = tourId,
            Name = "Query KP",
            Description = "Opis",
            ImageUrl = "",
            Secret = "Tajna",
            Latitude = 45.4,
            Longitude = 19.5
        });

        // Act
        var result = keyPointService.GetById(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.TourId.ShouldBe(tourId);
        result.Name.ShouldBe("Query KP");
    }

    [Fact]
    public void Fails_to_get_nonexistent_key_point()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();

        // Act & Assert
        Should.Throw<NotFoundException>(() => keyPointService.GetById(123456789)); // ID koji sigurno ne postoji

    }

    [Fact]
    public void Gets_paged_key_points()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var tourId = CreateTestTour(tourService);

        keyPointService.Create(new KeyPointDto
        {
            TourId = tourId,
            Name = "Paged KP 1",
            Description = "Opis 1",
            ImageUrl = "",
            Secret = "Tajna 1",
            Latitude = 45.0,
            Longitude = 19.8
        });

        keyPointService.Create(new KeyPointDto
        {
            TourId = tourId,
            Name = "Paged KP 2",
            Description = "Opis 2",
            ImageUrl = "",
            Secret = "Tajna 2",
            Latitude = 45.1,
            Longitude = 19.9
        });

        // Act
        var page = keyPointService.GetPaged(0, 10);

        // Assert
        page.ShouldNotBeNull();
        page.Results.ShouldNotBeNull();
        page.Results.Count.ShouldBeGreaterThanOrEqualTo(2);
        page.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
    }
}
