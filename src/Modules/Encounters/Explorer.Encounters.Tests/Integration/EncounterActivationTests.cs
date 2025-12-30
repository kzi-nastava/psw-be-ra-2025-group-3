using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class EncounterActivationTests : BaseEncountersIntegrationTest
{
    public EncounterActivationTests(EncountersTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public void GetNearbyEncounters_ReturnsEncountersWithDistance()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();
        var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

        long touristId = -21;

        // Postavi poziciju turiste na Petrovaradin Fortress (45.2517, 19.8658)
        positionService.Update(touristId, new PositionDto
        {
            TouristId = touristId,
            Latitude = 45.2517,
            Longitude = 19.8658
        });

        // Act
        var result = activationService.GetNearbyEncounters(touristId, 100);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);

        // Petrovaradin encounter bi trebao biti na 0m
        var petrovaradinEncounter = result.FirstOrDefault(e => e.Name.Contains("Petrovaradin"));
        petrovaradinEncounter.ShouldNotBeNull();
        petrovaradinEncounter.DistanceInMeters.ShouldBe(0, 1); // tolerance 1m
        petrovaradinEncounter.CanActivate.ShouldBeFalse();
        petrovaradinEncounter.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void ActivateEncounter_Success_WhenTouristIsCloseEnough()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();
        var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

        long touristId = -23;
        long encounterId = -1; // Petrovaradin Fortress

        // Postavi poziciju turiste na Petrovaradin (45.2517, 19.8658)
        positionService.Update(touristId, new PositionDto
        {
            TouristId = touristId,
            Latitude = 45.2517,
            Longitude = 19.8658
        });

        // Act
        var result = activationService.ActivateEncounter(touristId, encounterId);

        // Assert
        result.ShouldNotBeNull();
        result.EncounterId.ShouldBe(encounterId);
        result.TouristId.ShouldBe(touristId);
        result.Status.ShouldBe("InProgress");
        result.ActivatedAt.ShouldNotBe(default(DateTime));
        result.CompletedAt.ShouldBeNull();
    }

    [Fact]
    public void ActivateEncounter_Fails_WhenTouristIsTooFar()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();
        var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

        long touristId = -22; 
        long encounterId = -2; // Danube River Walk (45.2551, 19.8451)

        // Postavi poziciju turiste daleko (45.2671, 19.8335 - ~3km daleko)
        positionService.Update(touristId, new PositionDto
        {
            TouristId = touristId,
            Latitude = 45.2671,
            Longitude = 19.8335
        });

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            activationService.ActivateEncounter(touristId, encounterId)
        ).Message.ShouldContain("too far");
    }

    [Fact]
    public void ActivateEncounter_Fails_WhenAlreadyCompleted()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();
        var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

        long touristId = -21;
        long encounterId = -1; // Petrovaradin - već kompletiran u seed data

        // Postavi poziciju blizu
        positionService.Update(touristId, new PositionDto
        {
            TouristId = touristId,
            Latitude = 45.2517,
            Longitude = 19.8658
        });

        // Act & Assert - pokušaj ponovo da aktiviraš već kompletiran
        Should.Throw<InvalidOperationException>(() =>
            activationService.ActivateEncounter(touristId, encounterId)
        ).Message.ShouldContain("already completed");
    }

    [Fact]
    public void CompleteEncounter_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();

        long touristId = -23;
        long encounterId = -2; // Danube River Walk

        // Act
        var result = activationService.CompleteEncounter(touristId, encounterId);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Completed");
        result.CompletedAt.ShouldNotBeNull();
    }

    [Fact]
    public void CompleteEncounter_Fails_WhenNotActive()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();

        long touristId = -23; 
        long encounterId = -4; // Ovaj nije aktivan za -23

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            activationService.CompleteEncounter(touristId, encounterId)
        ).Message.ShouldContain("not active");
    }

    [Fact]
    public void AbandonEncounter_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();
        var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

        long touristId = -23; 
        long encounterId = -3; // Meet a Local Guide - aktivan za -23 u seed data

        // Act
        var result = activationService.AbandonEncounter(touristId, encounterId);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Failed");
        result.CompletedAt.ShouldNotBeNull();
    }


    [Fact]
    public void GetActiveEncounters_ReturnsOnlyInProgress()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();

        long touristId = -21; // Ima 1 aktivan iz seed data (-2: Danube River Walk)

        // Act
        var result = activationService.GetActiveEncounters(touristId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(1); // IZMENJENO - bar 1
        result.ShouldAllBe(a => a.Status == "InProgress");

        // Proveri da postoji Danube River Walk
        var danubeActivation = result.FirstOrDefault(a => a.EncounterId == -2);
        danubeActivation.ShouldNotBeNull();
    }

    [Fact]
    public void GetNearbyEncounters_Fails_WhenPositionNotSet()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var activationService = scope.ServiceProvider.GetRequiredService<IEncounterActivationService>();

        long touristId = -99;

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            activationService.GetNearbyEncounters(touristId, 100)
        ).Message.ShouldContain("position not found");
    }
}