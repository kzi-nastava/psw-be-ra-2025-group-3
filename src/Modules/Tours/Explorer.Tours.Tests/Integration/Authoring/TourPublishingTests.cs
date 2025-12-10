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
public class TourPublishingTests : BaseToursIntegrationTest
{
    public TourPublishingTests(ToursTestFactory factory) : base(factory) { }

    // Helper metoda za kreiranje osnovne ture (Clean Code princip iz tvojih primera)
    private TourDto CreateDraftTour(ITourService tourService, string name, List<string>? tags = null)
    {
        var tourDto = new TourCreateDto
        {
            Name = name,
            Description = $"Description for {name}",
            Difficulty = 0,
            Tags = tags ?? new List<string> { "test-tag" }
        };
        return tourService.Create(tourDto, -11);
    }

    [Fact]
    public void Publishes_tour_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. Kreiramo Draft turu
        var tourDto = CreateDraftTour(service, "Successful Publish Tour");

        // 2. Dodajemo uslove direktno u bazu (Simulacija unosa podataka)
        var tourEntity = dbContext.Tours.Find(tourDto.Id);

        // A) Trajanje
        tourEntity.UpdateTourDurations(new List<TourDuration>
        {
            new TourDuration(60, TransportType.Walking)
        });

        // B) Ključne tačke (Minimum 2)
        // Pazimo na konstruktor iz tvoje greške CS7036: (TourId, Name, Desc, Img, Secret, Lat, Long)
        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP1", "D1", "img1", "sec1", 45.0, 19.0));
        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP2", "D2", "img2", "sec2", 45.1, 19.1));


        dbContext.SaveChanges();

        // Act
        var result = service.Publish(tourDto.Id, -11);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1);
        result.PublishedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_to_publish_without_key_points()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourDto = CreateDraftTour(service, "No KeyPoints Tour");

        // Dodajemo samo Vreme, ali NE i tačke
        var tourEntity = dbContext.Tours.Find(tourDto.Id);
        tourEntity.UpdateTourDurations(new List<TourDuration>
        {
            new TourDuration(60, TransportType.Walking)
        });
        dbContext.SaveChanges();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Publish(tourDto.Id, -11)
        );
        exception.Message.ShouldContain("2 key points");
    }

    [Fact]
    public void Fails_to_publish_without_duration()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourDto = CreateDraftTour(service, "No Duration Tour");

        // Dodajemo Tačke, ali NE i vreme
        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP1", "D1", "img", "sec", 45.0, 19.0));
        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP2", "D2", "img", "sec", 45.1, 19.1));
        dbContext.SaveChanges();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Publish(tourDto.Id, -11)
        );
        exception.Message.ShouldContain("duration defined");
    }

    [Fact]
    public void Fails_to_publish_without_tags()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Kreiramo turu sa PRAZNIM tagovima
        var tourDto = CreateDraftTour(service, "No Tags Tour", new List<string>());

        // Dodajemo sve ostalo ispravno
        var tourEntity = dbContext.Tours.Find(tourDto.Id);
        tourEntity.UpdateTourDurations(new List<TourDuration> { new TourDuration(30, TransportType.Bicycle) });

        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP1", "D", "i", "s", 0, 0));
        dbContext.KeyPoints.Add(new KeyPoint(tourDto.Id, "KP2", "D", "i", "s", 0, 0));
        dbContext.SaveChanges();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            service.Publish(tourDto.Id, -11)
        );
        exception.Message.ShouldContain("Basic info is missing");
    }
}