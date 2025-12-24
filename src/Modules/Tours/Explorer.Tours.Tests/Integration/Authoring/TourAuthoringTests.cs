using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourAuthoringTests : BaseToursIntegrationTest
{
    public TourAuthoringTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour_with_default_status()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var newTour = new TourCreateDto
        {
            Name = "Nova Test Tura",
            Description = "Opis nove ture",
            Difficulty = 0, // Easy
            Tags = new List<string> { "priroda" }
        };

        // Act
        var result = service.Create(newTour, -11);

        // Assert - User Story 1
        result.ShouldNotBeNull();
        result.Status.ShouldBe(0); // 0 = Draft
    }

    [Fact]
    public void Adds_equipment_to_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // CREATE TOUR
        var createdTour = service.Create(new TourCreateDto
        {
            Name = "Equipment Test Tour",
            Description = "Test",
            Difficulty = 0,
            Tags = new List<string>()
        }, -11);

        // CREATE EQUIPMENT – OBAVEZNO PREKO KONSTRUKTORA
        var equipment = new Equipment("Test Equipment", "Desc");
        dbContext.Equipment.Add(equipment);
        dbContext.SaveChanges();

        // ACT
        var result = service.AddEquipment(createdTour.Id, equipment.Id, -11);

        // ASSERT
        result.Equipment.ShouldContain(e => e.Id == equipment.Id);
    }



    [Fact]
    public void Removes_equipment_from_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createdTour = service.Create(new TourCreateDto
        {
            Name = "Remove Equipment Tour",
            Description = "Test",
            Difficulty = 0,
            Tags = new List<string>()
        }, -11);

        // CREATE EQUIPMENT
        var equipment = new Equipment("Equipment", "Desc");
        dbContext.Equipment.Add(equipment);
        dbContext.SaveChanges();

        service.AddEquipment(createdTour.Id, equipment.Id, -11);

        // ACT
        var result = service.RemoveEquipment(createdTour.Id, equipment.Id, -11);

        // ASSERT
        result.Equipment.ShouldNotContain(e => e.Id == equipment.Id);
    }



    [Fact]
    public void Cannot_add_equipment_if_tour_archived()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. Kreiramo turu
        var tourDto = new TourCreateDto { Name = "Archived Test", Description = "Desc", Difficulty = 0, Tags = new List<string> { "test-tag" } };
        var createdTour = service.Create(tourDto, -11);

        // 2. Rucno je arhiviramo kroz bazu
        var tourEntity = dbContext.Tours.Find(createdTour.Id);

        tourEntity.UpdateTourDurations(new List<TourDuration>
        {
            new TourDuration(60, TransportType.Walking)
        });

        dbContext.KeyPoints.Add(new KeyPoint(createdTour.Id, "KP1", "Desc1", "http://img.com", "secret", 45.0, 19.0));
        dbContext.KeyPoints.Add(new KeyPoint(createdTour.Id, "KP2", "Desc2", "http://img.com", "secret", 45.1, 19.1));
        dbContext.SaveChanges();

        tourEntity.Publish();
        tourEntity.Archive();
        dbContext.SaveChanges();

        // Act & Assert
        Should.Throw<Exception>(() =>
            service.AddEquipment(createdTour.Id, -1, -11)
        );
    }
}