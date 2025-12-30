using Explorer.API.Controllers.Administrator;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Infrastructure.Database;
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
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var year = GetFreeYear(dbContext);

            var createDto = new AwardEventCreateDto
            {
                Name = "Query Test Award",
                Description = "For query test",
                Year = year,
                VotingStartDate = DateTime.UtcNow.AddDays(1),
                VotingEndDate = DateTime.UtcNow.AddDays(5)
            };

            controller.Create(createDto);

            var result =
                ((ObjectResult)controller.GetPaged(1, 50).Result)?.Value
                as PagedResult<AwardEventDto>;

            result.ShouldNotBeNull();
            result.Results.Any(e => e.Year == year).ShouldBeTrue();
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


        private static AwardEventController CreateController(IServiceScope scope)
        {
            return new AwardEventController(scope.ServiceProvider.GetRequiredService<IAwardEventService>())
            {
                ControllerContext = BuildContext("-1") //admin
            };
        }
    }
}