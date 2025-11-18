using Explorer.API.Controllers.Administrator;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class AwardEventQueryTests : BaseToursIntegrationTest 
    {
        public AwardEventQueryTests(ToursTestFactory factory) : base(factory) { } 

        [Fact]
        public void Retrieves_all()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetPaged(1, 50).Result)?.Value as PagedResult<AwardEventDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Results.Count.ShouldBeGreaterThan(0);
        }

        private static AwardEventController CreateController(IServiceScope scope)
        {
            return new AwardEventController(scope.ServiceProvider.GetRequiredService<IAwardEventService>())
            {
                ControllerContext = BuildContext("-1") //admin
            };
        }
    }
}