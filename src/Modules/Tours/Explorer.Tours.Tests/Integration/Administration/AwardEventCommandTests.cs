using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
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

            var existing = dbContext.AwardEvents.FirstOrDefault(e => e.Year == 2030);
            if (existing != null)
            {
                dbContext.AwardEvents.Remove(existing);
                dbContext.SaveChanges();
            }

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
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var invalidEntity = new AwardEventCreateDto
            {
                Name = "", // ❌ nevalidno
                Description = "Opis",
                Year = 2024,
                VotingStartDate = DateTime.UtcNow,
                VotingEndDate = DateTime.UtcNow.AddDays(5)
            };

            Should.Throw<ArgumentException>(() =>
                controller.Create(invalidEntity));
        }

        private static int GetFreeYear(ToursContext dbContext, int startYear = 2030)
        {
            var year = startYear;
            while (dbContext.AwardEvents.Any(e => e.Year == year))
            {
                year++;
            }
            return year;
        }


        [Fact]
        public void Updates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var year = GetFreeYear(dbContext);

            var createDto = new AwardEventCreateDto
            {
                Name = "Update Test Award",
                Description = "Original description",
                Year = year,
                VotingStartDate = DateTime.UtcNow.AddDays(1),
                VotingEndDate = DateTime.UtcNow.AddDays(5)
            };

            var created =
                ((ObjectResult)controller.Create(createDto).Result)?.Value as AwardEventDto;

            created.ShouldNotBeNull();

            var updateDto = new AwardEventUpdateDto
            {
                Id = created.Id,
                Name = "Ažurirana Nagrada",
                Description = "Promenjen opis",
                Year = year, // isto ostaje
                VotingStartDate = DateTime.UtcNow.AddDays(2),
                VotingEndDate = DateTime.UtcNow.AddDays(7)
            };

            var result =
                ((ObjectResult)controller.Update(created.Id, updateDto).Result)?.Value as AwardEventDto;

            result.ShouldNotBeNull();
            result.Name.ShouldBe(updateDto.Name);
        }



        [Fact]
        public void Deletes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var year = GetFreeYear(dbContext); // 🔑 BITNO

            var createDto = new AwardEventCreateDto
            {
                Name = "Delete Test Award",
                Description = "To be deleted",
                Year = year,
                VotingStartDate = DateTime.UtcNow.AddDays(1),
                VotingEndDate = DateTime.UtcNow.AddDays(5)
            };

            var created =
                ((ObjectResult)controller.Create(createDto).Result)?.Value as AwardEventDto;

            created.ShouldNotBeNull();

            var idToDelete = created.Id;

            // ACT
            var result = controller.Delete(idToDelete) as OkResult;

            // ASSERT
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            dbContext.AwardEvents.FirstOrDefault(e => e.Id == idToDelete)
                .ShouldBeNull();
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