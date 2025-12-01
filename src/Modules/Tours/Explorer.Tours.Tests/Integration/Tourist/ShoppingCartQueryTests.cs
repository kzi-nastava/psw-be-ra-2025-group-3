using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class ShoppingCartQueryTests : BaseToursIntegrationTest
    {
        public ShoppingCartQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_returns_current_cart()
        {
            // Arrange
            var personId = "-22";   
            var tourId = -2;        
            var expectedPrice = 500m;

            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var addRequest = new ShoppingCartRequestDto
            {
                TourId = tourId
            };

            controller.Add(addRequest);

            var result = ((ObjectResult)controller.Get().Result)?.Value as ShoppingCartDto;

            // Assert 
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(long.Parse(personId));
            result.Items.Count.ShouldBe(1);
            result.Items[0].TourId.ShouldBe(tourId);
            result.Items[0].Price.ShouldBe(expectedPrice);
            result.TotalPrice.ShouldBe(expectedPrice);

            // Assert 
            var storedCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.TouristId == long.Parse(personId));
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items[0].TourId.ShouldBe(tourId);
            storedCart.TotalPrice.ShouldBe(expectedPrice);
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

