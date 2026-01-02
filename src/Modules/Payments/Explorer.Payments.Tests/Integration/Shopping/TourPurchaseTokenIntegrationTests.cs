using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Shopping;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration.Shopping
{
    [Collection("Sequential")]
    public class TourPurchaseTokenIntegrationTests : BasePaymentsIntegrationTest
    {
        public TourPurchaseTokenIntegrationTests(PaymentsTestFactory factory)
            : base(factory) { }

        private static string NewPersonId()
        {
            return (-10000 - Guid.NewGuid().GetHashCode()).ToString();
        }

        [Fact]
        public void Checkout_creates_tokens_and_empties_cart()
        {
            // Arrange
            var personId = NewPersonId();
            var touristId = long.Parse(personId);
            var tourId = -2;

            using var scope = Factory.Services.CreateScope();

            var cartController = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };

            var purchaseController = new TourPurchaseController(
                scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>())
            {
                ControllerContext = BuildContext(personId)
            };

            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Add item to cart
            cartController.Add(new ShoppingCartRequestDto { TourId = tourId });

            // Act – perform checkout
            var result = ((ObjectResult)purchaseController.Checkout().Result)?.Value
                         as List<TourPurchaseTokenDto>;

            // Assert – returned tokens
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].TourId.ShouldBe(tourId);
            result[0].TouristId.ShouldBe(touristId);

            // Assert – token exists in DB
            var stored = db.TourPurchaseTokens
                            .FirstOrDefault(t => t.TouristId == touristId && t.TourId == tourId);

            stored.ShouldNotBeNull();
            stored.Token.ShouldBe(result[0].Token);

            // Assert – cart is emptied
            var storedCart = db.ShoppingCarts.First(c => c.TouristId == touristId);
            storedCart.Items.Count.ShouldBe(0);
            storedCart.TotalPrice.ShouldBe(0);
        }

        [Fact]
        public void GetTokens_returns_all_tokens_for_tourist()
        {
            // Arrange
            var personId = NewPersonId();
            var touristId = long.Parse(personId);
            var tour1 = -2;
            var tour2 = -4;

            using var scope = Factory.Services.CreateScope();

            var cart = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            { ControllerContext = BuildContext(personId) };

            var purchase = new TourPurchaseController(
                scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>())
            { ControllerContext = BuildContext(personId) };

            // Add two tours
            cart.Add(new ShoppingCartRequestDto { TourId = tour1 });
            cart.Add(new ShoppingCartRequestDto { TourId = tour2 });

            // Checkout
            purchase.Checkout();

            // Act – get tokens
            var result = ((ObjectResult)purchase.GetTokens().Result)?.Value
                         as List<TourPurchaseTokenDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.Select(t => t.TourId).ShouldContain(tour1);
            result.Select(t => t.TourId).ShouldContain(tour2);
        }
    }
}
