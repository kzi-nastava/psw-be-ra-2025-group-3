using System.Linq;
using Explorer.API.Controllers.Tourist.Position;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class PositionCommandTests : BaseToursIntegrationTest
    {
        public PositionCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Updates_or_creates_position()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // 1. Očisti prethodne pozicije turista -11
            var oldPositions = dbContext.Positions.Where(p => p.TouristId == -21).ToList();
            if (oldPositions.Any())
            {
                dbContext.Positions.RemoveRange(oldPositions);
                dbContext.SaveChanges();
            }

            // 2. Kreiraj controller za turist -11 (postojeći user iz stakeholders)
            var controller = CreateController(scope, "-21");

            // 3. DTO sa željenim koordinatama
            var dto = new PositionDto
            {
                Latitude = 45.2500,
                Longitude = 19.8300
            };

            // 4. Poziv metode update
            var result = controller.Update(dto) as OkResult;

            result.ShouldNotBeNull();
            result!.StatusCode.ShouldBe(200);

            // 5. Proveri da li je nova pozicija kreirana sa ispravnim vrednostima
            var entity = dbContext.Positions.FirstOrDefault(p => p.TouristId == -21);
            entity.ShouldNotBeNull();
            entity.Latitude.ShouldBe(45.2500);  // sada neće biti starih vrednosti
            entity.Longitude.ShouldBe(19.8300);
        }


        private static PositionController CreateController(IServiceScope scope, string touristId)
        {
            return new PositionController(scope.ServiceProvider.GetRequiredService<IPositionService>())
            {
                ControllerContext = BuildContext(touristId)
            };
        }

        private static ControllerContext BuildContext(string touristId)
        {
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", touristId)
                }, "mock"));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
