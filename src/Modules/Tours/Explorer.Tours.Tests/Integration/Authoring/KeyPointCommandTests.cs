using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class KeyPointCommandTests : BaseToursIntegrationTest
{
    public KeyPointCommandTests(ToursTestFactory factory) : base(factory) { }

    private (IKeyPointService keyPointService, ITourService tourService) GetServices()
    {
        var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        return (keyPointService, tourService);
    }

    private long CreateTestTour(ITourService tourService)
    {
        var tourDto = new TourCreateDto
        {
            Name = "KeyPoint Test Tour",
            Description = "Tour for key point tests",
            Difficulty = 1,
            Tags = new List<string> { "kp-test" }
        };

        var created = tourService.Create(tourDto, -11); // isti author kao u tvojim testovima
        created.Id.ShouldNotBe(0);
        return created.Id;
    }

    [Fact]
    public void Creates_key_point()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var tourId = CreateTestTour(tourService);

        var dto = new KeyPointDto
        {
            TourId = tourId,
            Name = "Test key point",
            Description = "Opis key pointa",
            ImageUrl = "http://example.com/image.jpg",
            Secret = "Tajna priča",
            Latitude = 45.0,
            Longitude = 19.8
        };

        // Act
        var result = keyPointService.Create(dto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(tourId);
        result.Name.ShouldBe(dto.Name);
        result.Description.ShouldBe(dto.Description);
        result.ImageUrl.ShouldBe(dto.ImageUrl);
        result.Secret.ShouldBe(dto.Secret);
        result.Latitude.ShouldBe(dto.Latitude);
        result.Longitude.ShouldBe(dto.Longitude);
    }

    [Fact]
    public void Updates_key_point()
    {
        long tourId;
        long keyPointId;

        // 1. scope – kreiramo turu i key point
        using (var scope = Factory.Services.CreateScope())
        {
            var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

            tourId = CreateTestTour(tourService);

            var created = keyPointService.Create(new KeyPointDto
            {
                TourId = tourId,
                Name = "Staro ime",
                Description = "Stari opis",
                ImageUrl = "http://example.com/old.jpg",
                Secret = "Stara tajna",
                Latitude = 45.1,
                Longitude = 19.9
            });

            keyPointId = created.Id;
        }

        // 2. scope – radimo update (novi DbContext, nema konflikta)
        using var scope2 = Factory.Services.CreateScope();
        var keyPointService2 = scope2.ServiceProvider.GetRequiredService<IKeyPointService>();

        var updateDto = new KeyPointDto
        {
            Id = keyPointId,
            TourId = tourId,
            Name = "Novo ime",
            Description = "Novi opis",
            ImageUrl = "http://example.com/new.jpg",
            Secret = "Nova tajna",
            Latitude = 45.1,
            Longitude = 19.9
        };

        // Act
        var updated = keyPointService2.Update(updateDto);

        // Assert
        updated.ShouldNotBeNull();
        updated.Id.ShouldBe(keyPointId);
        updated.TourId.ShouldBe(tourId);
        updated.Name.ShouldBe("Novo ime");
        updated.Description.ShouldBe("Novi opis");
        updated.ImageUrl.ShouldBe("http://example.com/new.jpg");
        updated.Secret.ShouldBe("Nova tajna");
    }


    [Fact]
    public void Deletes_key_point()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var tourId = CreateTestTour(tourService);

        var created = keyPointService.Create(new KeyPointDto
        {
            TourId = tourId,
            Name = "Za brisanje",
            Description = "Opis",
            ImageUrl = "",
            Secret = "Tajna",
            Latitude = 45.2,
            Longitude = 19.7
        });

        // Act
        keyPointService.Delete(created.Id);

        // Assert
        Should.Throw<NotFoundException>(() => keyPointService.GetById(created.Id));
    }


    [Fact]
    public void Gets_paged_key_points()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var keyPointService = scope.ServiceProvider.GetRequiredService<IKeyPointService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var tourId = CreateTestTour(tourService);

        // bar jedan zapis
        keyPointService.Create(new KeyPointDto
        {
            TourId = tourId,
            Name = "Paged KP",
            Description = "Opis",
            ImageUrl = "",
            Secret = "Tajna",
            Latitude = 45.3,
            Longitude = 19.6
        });

        // Act
        var page = keyPointService.GetPaged(0, 10);

        // Assert
        page.ShouldNotBeNull();
        page.Results.ShouldNotBeNull();
        page.Results.Count.ShouldBeGreaterThan(0);
        page.TotalCount.ShouldBeGreaterThan(0);
    }
}
