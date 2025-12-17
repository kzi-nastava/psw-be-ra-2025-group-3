using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class ShoppingCartCommandTests : BaseToursIntegrationTest
    {
        public ShoppingCartCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Adds_published_tour_to_cart()
        {
            // Arrange
            var personId = "-21";   
            var tourId = -2;        
            var expectedTotal = 500m;

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var request = new ShoppingCartRequestDto
            {
                TourId = tourId
            };

            // Act
            var result = ((ObjectResult)controller.Add(request).Result)?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.TotalPrice.ShouldBe(expectedTotal);
            result.Items.Count.ShouldBe(1);
            result.Items[0].TourId.ShouldBe(tourId);

            // Assert
            var touristIdLong = long.Parse(personId);
            var storedCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.TouristId == touristIdLong);
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items[0].TourId.ShouldBe(tourId);
        }

        [Fact]
        public void Add_fails_for_non_published_tour()
        {
            // Arrange
            var personId = "-21";
            var tourId = -1;    

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);

            var request = new ShoppingCartRequestDto
            {
                TourId = tourId
            };

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => controller.Add(request));
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string personId)
        {
            return new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}
