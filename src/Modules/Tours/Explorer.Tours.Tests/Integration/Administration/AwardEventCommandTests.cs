using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class AwardEventCommandTests : BaseToursIntegrationTest 
    {
        public AwardEventCommandTests(ToursTestFactory factory) : base(factory) { } 

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var newEntity = new AwardEventCreateDto
            {
                Name = "Nova Test Nagrada",
                Description = "Opis nove nagrade",
                Year = 2030,
                VotingStartDate = DateTime.UtcNow.AddDays(1),
                VotingEndDate = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as AwardEventDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newEntity.Name);

            // Provera u bazi
            var storedEntity = dbContext.AwardEvents.FirstOrDefault(i => i.Name == newEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var invalidEntity = new AwardEventCreateDto
            {
                Name = "Lose Godine",
                Description = "Opis",
                Year = 2024,
                VotingStartDate = DateTime.UtcNow,
                VotingEndDate = DateTime.UtcNow.AddDays(5)
            };

            var exception = Should.Throw<InvalidOperationException>(() => controller.Create(invalidEntity));
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var updateEntity = new AwardEventUpdateDto
            {
                Id = -1,
                Name = "Ažurirana Nagrada 2024",
                Description = "Promenjen opis",
                Year = 2024,
                VotingStartDate = DateTime.UtcNow.AddDays(1),
                VotingEndDate = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var result = ((ObjectResult)controller.Update(-1, updateEntity).Result)?.Value as AwardEventDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Name.ShouldBe(updateEntity.Name);

            var storedEntity = dbContext.AwardEvents.FirstOrDefault(i => i.Id == -1);
            storedEntity.ShouldNotBeNull();
            storedEntity.Description.ShouldBe(updateEntity.Description);
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = (OkResult)controller.Delete(-2);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var storedEntity = dbContext.AwardEvents.FirstOrDefault(i => i.Id == -2);
            storedEntity.ShouldBeNull();
        }

        private static AwardEventController CreateController(IServiceScope scope)
        {
            return new AwardEventController(scope.ServiceProvider.GetRequiredService<IAwardEventService>())
            {
                ControllerContext = BuildContext("-1") // admin
            };
        }
    }
}