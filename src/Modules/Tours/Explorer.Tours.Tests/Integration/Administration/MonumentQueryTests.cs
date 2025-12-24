using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentQueryTests : BaseToursIntegrationTest
{
    public MonumentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // ARRANGE – stanje PRE
        var beforeResult =
            ((ObjectResult)controller.GetAll(0, 0).Result)?.Value
            as PagedResult<MonumentDto>;

        beforeResult.ShouldNotBeNull();
        var beforeCount = beforeResult.Results.Count;

        // dodajemo 3 nova monumenta
        controller.Create(new MonumentDto
        {
            Name = "M1",
            Description = "D1",
            Year = 2000,
            Status = 1,
            Latitude = 45,
            Longitude = 19
        });

        controller.Create(new MonumentDto
        {
            Name = "M2",
            Description = "D2",
            Year = 2001,
            Status = 1,
            Latitude = 46,
            Longitude = 20
        });

        controller.Create(new MonumentDto
        {
            Name = "M3",
            Description = "D3",
            Year = 2002,
            Status = 0,
            Latitude = 47,
            Longitude = 21
        });

        // ACT – stanje POSLE
        var afterResult =
            ((ObjectResult)controller.GetAll(0, 0).Result)?.Value
            as PagedResult<MonumentDto>;

        // ASSERT
        afterResult.ShouldNotBeNull();
        afterResult.Results.Count.ShouldBe(beforeCount + 3);
    }



    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}