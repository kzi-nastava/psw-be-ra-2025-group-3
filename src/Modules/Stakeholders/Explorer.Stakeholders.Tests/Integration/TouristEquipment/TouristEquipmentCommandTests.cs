using Explorer.API.Controllers.Tourist.EquipmentInventory;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.TouristEquipment;

[Collection("Sequential")]
public class TouristEquipmentCommandTests : BaseStakeholdersIntegrationTest
{
    public TouristEquipmentCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Adds_equipment_to_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        long equipmentId = -3;

        // Act
        var result = ((ObjectResult)controller.AddEquipment(equipmentId).Result)?.Value as EquipmentWithOwnershipDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-3);
        result.Name.ShouldBe("Obična baterijska lampa");
        result.IsOwnedByTourist.ShouldBeTrue();

        var tourist = dbContext.Tourists.FirstOrDefault(t => t.PersonId == -21);
        tourist.ShouldNotBeNull();
        tourist.EquipmentIds.ShouldContain(-3);
    }

    [Fact]
    public void Add_fails_when_equipment_already_owned()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);

        long equipmentId = -1;

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.AddEquipment(equipmentId));
    }

    [Fact]
    public void Deletes_equipment_from_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        long equipmentId = -2;

        // Act
        var result = controller.DeleteEquipment(equipmentId);

        // Assert
        result.ShouldNotBeNull();
        ((NoContentResult)result).StatusCode.ShouldBe(204);

        var tourist = dbContext.Tourists.FirstOrDefault(t => t.PersonId == -21);
        tourist.ShouldNotBeNull();
        tourist.EquipmentIds.ShouldNotContain(-2);
    }

    [Fact]
    public void Delete_fails_when_equipment_not_owned()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerWithMockEquipment(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // PRVO proveri da turista NE IMA opremu -3
        var tourist = dbContext.Tourists.FirstOrDefault(t => t.PersonId == -21);
        if (tourist != null && tourist.EquipmentIds.Contains(-3))
        {
            // Ako ima -3 (od prethodnog testa), ukloni je
            tourist.EquipmentIds.Remove(-3);
            dbContext.SaveChanges();
        }

        long equipmentId = -3;

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.DeleteEquipment(equipmentId));
    }

    private static TouristEquipmentController CreateControllerWithMockEquipment(IServiceScope scope)
    {
        // MOCK - koristi se SAMO u testovima!
        var mockEquipmentService = new Mock<IInternalEquipmentService>();

        mockEquipmentService.Setup(s => s.GetAll())
            .Returns(new List<EquipmentDto>
            {
                new EquipmentDto { Id = -1, Name = "Voda", Description = "Količina vode varira..." },
                new EquipmentDto { Id = -2, Name = "Štapovi za šetanje", Description = "Štapovi umanjuju..." },
                new EquipmentDto { Id = -3, Name = "Obična baterijska lampa", Description = "Baterijska lampa..." }
            });

        mockEquipmentService.Setup(s => s.Get(-1))
            .Returns(new EquipmentDto { Id = -1, Name = "Voda", Description = "Količina vode varira..." });
        mockEquipmentService.Setup(s => s.Get(-2))
            .Returns(new EquipmentDto { Id = -2, Name = "Štapovi za šetanje", Description = "Štapovi umanjuju..." });
        mockEquipmentService.Setup(s => s.Get(-3))
            .Returns(new EquipmentDto { Id = -3, Name = "Obična baterijska lampa", Description = "Baterijska lampa..." });

        var touristRepository = scope.ServiceProvider.GetRequiredService<ITouristRepository>();
        var service = new TouristEquipmentService(touristRepository, mockEquipmentService.Object);

        return new TouristEquipmentController(service)
        {
            ControllerContext = BuildContext("-21")
        };
    }
}