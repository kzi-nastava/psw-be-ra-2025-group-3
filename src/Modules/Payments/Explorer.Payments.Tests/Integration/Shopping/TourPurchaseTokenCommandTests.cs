
using Explorer.API.Controllers.Shopping;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPurchaseTokenCommandTests : BasePaymentsIntegrationTest
{
    public TourPurchaseTokenCommandTests(PaymentsTestFactory factory) : base(factory) { }

    private static string NewPersonId() => (-10000 - Guid.NewGuid().GetHashCode()).ToString();

    [Fact]
    public void Checkout_creates_single_token()
    {
        var personId = NewPersonId();
        var tourId = -2;

        using var scope = Factory.Services.CreateScope();
        var cart = CreateCartController(scope, personId);
        var purchase = CreatePurchaseController(scope, personId);

        cart.Add(new ShoppingCartRequestDto { TourId = tourId });

        var actionResult = purchase.Checkout();
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        var result = okResult.Value as CheckoutResultDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Tokens.Count.ShouldBe(1);
        result.Tokens[0].TourId.ShouldBe(tourId);
    }

    [Fact]
    public void Checkout_empties_cart()
    {
        var personId = NewPersonId();
        var tourId = -2;

        using var scope = Factory.Services.CreateScope();
        var cartController = CreateCartController(scope, personId);
        var purchaseController = CreatePurchaseController(scope, personId);
        var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        cartController.Add(new ShoppingCartRequestDto { TourId = tourId });

        var actionResult = purchaseController.Checkout();
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        var checkoutResult = okResult.Value as CheckoutResultDto;
        checkoutResult.ShouldNotBeNull();
        checkoutResult.Success.ShouldBeTrue();

        var storedCart = db.ShoppingCarts.First(c => c.TouristId == long.Parse(personId));
        storedCart.Items.Count.ShouldBe(0);
        storedCart.TotalPrice.ShouldBe(0);
    }

    [Fact]
    public void Checkout_fails_if_empty()
    {
        var personId = NewPersonId();

        using var scope = Factory.Services.CreateScope();
        var purchaseController = CreatePurchaseController(scope, personId);

        // Prazna korpa ne baca exception više, vraća BadRequest
        var actionResult = purchaseController.Checkout();
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static ShoppingCartController CreateCartController(IServiceScope scope, string personId) =>
        new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
        { ControllerContext = BuildContext(personId) };

    private static TourPurchaseController CreatePurchaseController(IServiceScope scope, string personId) =>
        new TourPurchaseController(scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>())
        { ControllerContext = BuildContext(personId) };
}