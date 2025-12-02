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
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // 1. Kreiramo svezu turu da ne zavisimo od ID-a -1
        var tourDto = new TourCreateDto
        {
            Name = "Equipment Test Tour",
            Description = "Test",
            Difficulty = 0,
            Tags = new List<string>()
        };
        var createdTour = service.Create(tourDto, -11); // -11 je autor

        long equipmentId = -1; // Oprema -1 bi trebalo da postoji iz b-equipment.sql

        // Act
        var result = service.AddEquipment(createdTour.Id, equipmentId, -11);

        // Assert - User Story 2
        result.ShouldNotBeNull();
        result.Equipment.ShouldContain(e => e.Id == equipmentId);
    }

    [Fact]
    public void Removes_equipment_from_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();

        // 1. Kreiramo turu
        var tourDto = new TourCreateDto
        {
            Name = "Remove Equipment Tour",
            Description = "Test",
            Difficulty = 0,
            Tags = new List<string>()
        };
        var createdTour = service.Create(tourDto, -11);
        long equipmentId = -1;

        // 2. Dodajemo opremu (priprema za brisanje)
        service.AddEquipment(createdTour.Id, equipmentId, -11);

        // Act
        var result = service.RemoveEquipment(createdTour.Id, equipmentId, -11);

        // Assert - User Story 2
        result.ShouldNotBeNull();
        result.Equipment.ShouldNotContain(e => e.Id == equipmentId);
    }

    [Fact]
    public void Cannot_add_equipment_if_tour_archived()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. Kreiramo turu
        var tourDto = new TourCreateDto { Name = "Archived Test", Description = "Desc", Difficulty = 0, Tags = new List<string>() };
        var createdTour = service.Create(tourDto, -11);

        // 2. Rucno je arhiviramo kroz bazu
        var tourEntity = dbContext.Tours.Find(createdTour.Id);
        tourEntity.Publish();
        tourEntity.Archive();
        dbContext.SaveChanges();

        // Act & Assert
        Should.Throw<Exception>(() =>
            service.AddEquipment(createdTour.Id, -1, -11)
        );
    }
}