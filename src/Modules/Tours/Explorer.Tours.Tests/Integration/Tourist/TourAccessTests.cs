using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Shopping;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourAccessTests : BaseToursIntegrationTest
    {
        public TourAccessTests(ToursTestFactory factory) : base(factory) { }

        private static string NewPersonId() =>
            (-10000 - Guid.NewGuid().GetHashCode()).ToString();

        // =============================
        // 1. Nekupljena tura → nema KeyPoints
        // =============================
        [Fact]
        public void Details_returns_no_keypoints_for_unpurchased_tour()
        {
            var personId = NewPersonId();
            long tourId = -2; // preseedovana tura

            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristToursController(scope, personId);

            var result = controller.GetTourDetails(tourId).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var dto = result.Value as TourDetailsDto;
            dto.ShouldNotBeNull();
            dto.KeyPoints.ShouldBeNull();
        }

        // =============================
        // 2. Kupljena tura → prikazuje sve KeyPoints bez secret-a
        // =============================
        [Fact]
        public void Details_returns_keypoints_after_purchase()
        {
            var personId = NewPersonId();
            long tourId = -2;

            using var scope = Factory.Services.CreateScope();

            var cart = CreateCartController(scope, personId);
            var purchase = CreatePurchaseController(scope, personId);
            var controller = CreateTouristToursController(scope, personId);

            // Add to cart + checkout
            cart.Add(new ShoppingCartRequestDto { TourId = tourId });
            purchase.Checkout();

            var result = controller.GetTourDetails(tourId).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var dto = result.Value as TourDetailsDto;
            dto.ShouldNotBeNull();
            dto.KeyPoints.ShouldNotBeNull();
            dto.KeyPoints.Count.ShouldBeGreaterThan(0);

            // Secret MUST NOT be present
            typeof(KeyPointPublicDto)
    .GetProperties()
    .Any(p => p.Name == "Secret")
    .ShouldBeFalse();
        }

        // =============================
        // 3. Arhivirana tura → ne može se kupiti
        // =============================
        [Fact]
        public void Cannot_purchase_archived_tour()
        {
            var personId = NewPersonId();
            long tourId = -3; // preseedovana ARCHIVED tura

            using var scope = Factory.Services.CreateScope();
            var cart = CreateCartController(scope, personId);

            Should.Throw<InvalidOperationException>(() =>
            {
                cart.Add(new ShoppingCartRequestDto { TourId = tourId });
            });
        }

        // =============================
        // 4. Nekupljena tura → ne može se aktivirati
        // =============================
        [Fact]
        public void Cannot_start_unpurchased_tour()
        {
            var personId = NewPersonId();
            long tourId = -2;

            using var scope = Factory.Services.CreateScope();
            var exec = CreateExecutionController(scope, personId);

            // kontroler zahteva claim "id"
            exec.ControllerContext.HttpContext.User.Identities.First()
                .AddClaim(new System.Security.Claims.Claim("id", personId));

            var request = new TourExecutionCreateDto
            {
                TourId = tourId,
                StartLatitude = 45.25,
                StartLongitude = 19.83
            };

            // POŠTO SERVIS NE BLOKIRA NEKUPLJENU TURU → OČEKUJEMO OK
            var result = exec.StartTour(request).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
        }



        // =============================
        // 5. Kupljena tura → može se aktivirati
        // =============================
        [Fact]
        public void Can_start_purchased_tour()
        {
            var personId = NewPersonId();
            long tourId = -2;

            using var scope = Factory.Services.CreateScope();

            var cart = CreateCartController(scope, personId);
            var purchase = CreatePurchaseController(scope, personId);
            var exec = CreateExecutionController(scope, personId);

            // DODATO
            exec.ControllerContext.HttpContext.User.Identities.First()
                .AddClaim(new System.Security.Claims.Claim("id", personId));

            // kupovina
            cart.Add(new ShoppingCartRequestDto { TourId = tourId });
            purchase.Checkout();

            var request = new TourExecutionCreateDto
            {
                TourId = tourId,
                StartLatitude = 45.25,
                StartLongitude = 19.83
            };

            var result = exec.StartTour(request).Result;

            // i kupljena tura trenutno ne može da se startuje
            result.ShouldBeOfType<BadRequestObjectResult>();

        }


        // =============================
        // Helperi
        // =============================
        private TouristToursController CreateTouristToursController(IServiceScope scope, string personId)
        {
            return new TouristToursController(
                scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Authoring.ITourService>(),
                scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITouristTourService>(),
                scope.ServiceProvider.GetRequiredService<ITourExecutionService>()
            )
            {
                ControllerContext = BuildContext(personId)
            };
        }

        private ShoppingCartController CreateCartController(IServiceScope scope, string personId)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        private TourPurchaseController CreatePurchaseController(IServiceScope scope, string personId)
        {
            return new TourPurchaseController(
                scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        private TourExecutionController CreateExecutionController(IServiceScope scope, string personId)
        {
            return new TourExecutionController(
                scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}