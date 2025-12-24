using System;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourAccessTests : BaseToursIntegrationTest
    {
        public TourAccessTests(ToursTestFactory factory) : base(factory) { }

        private static string NewPersonId() =>
            (-10000 - Guid.NewGuid().GetHashCode()).ToString();

        // =============================
        // 1. Ne postoji tura → NotFound
        // =============================
        [Fact]
        public void Details_returns_not_found_for_non_existing_tour()
        {
            var personId = NewPersonId();
            long tourId = -2;

            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristToursController(scope, personId);

            Should.Throw<NotFoundException>(() =>
            {
                controller.GetTourDetails(tourId);
            });
        }

        // =============================
        // 2. Ne postoji tura → ne može start
        // =============================
        [Fact]
        public void Cannot_start_non_existing_tour()
        {
            var personId = NewPersonId();
            long tourId = -2;

            using var scope = Factory.Services.CreateScope();
            var exec = CreateExecutionController(scope, personId);

            exec.ControllerContext.HttpContext.User.Identities.First()
                .AddClaim(new System.Security.Claims.Claim("id", personId));

            var request = new TourExecutionCreateDto
            {
                TourId = tourId,
                StartLatitude = 45.25,
                StartLongitude = 19.83
            };

            Should.Throw<NotFoundException>(() =>
            {
                exec.StartTour(request);
            });
        }

       
        // =============================
        // Helperi
        // =============================
        private TouristToursController CreateTouristToursController(IServiceScope scope, string personId)
        {
            return new TouristToursController(
                scope.ServiceProvider.GetRequiredService<
                    Explorer.Tours.API.Public.Authoring.ITourService>(),
                scope.ServiceProvider.GetRequiredService<
                    Explorer.Tours.API.Public.Tourist.ITouristTourService>(),
                scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
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
