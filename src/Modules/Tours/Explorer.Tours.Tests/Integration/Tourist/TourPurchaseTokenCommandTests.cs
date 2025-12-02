using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPurchaseTokenCommandTests : BaseToursIntegrationTest
{
    public TourPurchaseTokenCommandTests(ToursTestFactory factory) : base(factory) { }

    // Helper – generiše novog unikatnog turistu
    private static string NewPersonId()
    {
        return (-10000 - Guid.NewGuid().GetHashCode()).ToString();
    }

    [Fact]
    public void Checkout_creates_single_token()
    {
        var personId = NewPersonId();
        var tourId = -2;

        using var scope = Factory.Services.CreateScope();
        var cart = CreateCartController(scope, personId);
        var purchase = CreatePurchaseController(scope, personId);

        cart.Add(new ShoppingCartRequestDto { TourId = tourId });

        var result = ((ObjectResult)purchase.Checkout().Result)?.Value
                     as List<TourPurchaseTokenDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].TourId.ShouldBe(tourId);
    }


    [Fact]
    public void Checkout_empties_cart()
    {
        var personId = NewPersonId();
        var tourId = -2;

        using var scope = Factory.Services.CreateScope();
        var cartController = CreateCartController(scope, personId);
        var purchaseController = CreatePurchaseController(scope, personId);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        cartController.Add(new ShoppingCartRequestDto { TourId = tourId });

        purchaseController.Checkout();

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

        Should.Throw<InvalidOperationException>(() => purchaseController.Checkout());
    }

    private static ShoppingCartController CreateCartController(IServiceScope scope, string personId)
    {
        return new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static TourPurchaseController CreatePurchaseController(IServiceScope scope, string personId)
    {
        return new TourPurchaseController(scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}
