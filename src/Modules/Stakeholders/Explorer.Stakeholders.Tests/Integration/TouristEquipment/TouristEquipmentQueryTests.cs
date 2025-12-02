using Explorer.API.Controllers.Tourist.EquipmentInventory;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.TouristEquipment;

[Collection("Sequential")]
public class TouristEquipmentQueryTests : BaseStakeholdersIntegrationTest
{
    public TouristEquipmentQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_equipment_with_ownership()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllEquipmentWithOwnership().Result)?.Value as List<EquipmentWithOwnershipDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        var ownedEquipment = result.Where(e => e.IsOwnedByTourist).ToList();
        ownedEquipment.Count.ShouldBe(2);
        ownedEquipment.ShouldContain(e => e.Id == -1 && e.Name == "Voda");
        ownedEquipment.ShouldContain(e => e.Id == -2 && e.Name == "Štapovi za šetanje");

        var notOwnedEquipment = result.Where(e => !e.IsOwnedByTourist).ToList();
        notOwnedEquipment.Count.ShouldBe(1);
        notOwnedEquipment.ShouldContain(e => e.Id == -3 && e.Name == "Obična baterijska lampa");
    }

    [Fact]
    public void Retrieves_only_tourist_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyEquipment().Result)?.Value as List<EquipmentWithOwnershipDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(e => e.IsOwnedByTourist == true);

        result.ShouldContain(e => e.Id == -1 && e.Name == "Voda");
        result.ShouldContain(e => e.Id == -2 && e.Name == "Štapovi za šetanje");
    }

    private static TouristEquipmentController CreateControllerWithMockEquipment(IServiceScope scope)
    {
        // MOCK - koristi se SAMO u testovima!
        // U produkciji se koristi pravi InternalEquipmentService koji čita iz baze
        var mockEquipmentService = new Mock<IInternalEquipmentService>();

        mockEquipmentService.Setup(s => s.GetAll())
            .Returns(new List<EquipmentDto>
            {
                new EquipmentDto { Id = -1, Name = "Voda", Description = "Količina vode varira..." },
                new EquipmentDto { Id = -2, Name = "Štapovi za šetanje", Description = "Štapovi umanjuju..." },
                new EquipmentDto { Id = -3, Name = "Obična baterijska lampa", Description = "Baterijska lampa..." }
            });

        var touristRepository = scope.ServiceProvider.GetRequiredService<ITouristRepository>();
        var service = new TouristEquipmentService(touristRepository, mockEquipmentService.Object);

        return new TouristEquipmentController(service)
        {
            ControllerContext = BuildContext("-21")
        };
    }
}