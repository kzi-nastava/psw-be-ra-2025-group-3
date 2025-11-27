using System.Linq;
using Explorer.API.Controllers.Tourist.Position;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class PositionQueryTests : BaseToursIntegrationTest
    {
        public PositionQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_position_by_tourist_id()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Seed pozicija za test turista -12 ako ne postoji
            if (!db.Positions.Any(p => p.TouristId == -12))
            {
                db.Positions.Add(new Position
                {
                    TouristId = -12,
                    Latitude = 44.7872,
                    Longitude = 20.4573
                });
                db.SaveChanges();
            }

            var controller = CreateController(scope, "-12");

            var actionResult = controller.GetMyPosition();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as PositionDto;

            result.ShouldNotBeNull();
            result!.TouristId.ShouldBe(-12);
            result.Latitude.ShouldBe(44.7872);
            result.Longitude.ShouldBe(20.4573);
        }

        [Fact]
        public void Returns_null_when_position_does_not_exist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-50");

            var actionResult = controller.GetMyPosition();
            var result = actionResult.Value;

            result.ShouldBeNull();
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
