using Explorer.API.Controllers.Shopping;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Payments.Tests.Integration;

namespace Explorer.Payments.Tests.Integration.Shopping;

[Collection("Sequential")]
public class TourPurchaseTokenQueryTests : BasePaymentsIntegrationTest
{
    public TourPurchaseTokenQueryTests(PaymentsTestFactory factory) : base(factory) { }

    private static string NewPersonId()
    {
        return (-10000 - Guid.NewGuid().GetHashCode()).ToString();
    }

    [Fact]
    public void GetTokens_returns_all_for_tourist()
    {
        var personId = NewPersonId();
        var tour1 = -2;
        var tour2 = -4;

        using var scope = Factory.Services.CreateScope();
        var cart = CreateCartController(scope, personId);
        var purchase = CreatePurchaseController(scope, personId);

        cart.Add(new ShoppingCartRequestDto { TourId = tour1 });
        cart.Add(new ShoppingCartRequestDto { TourId = tour2 });

        purchase.Checkout();

        var result = ((ObjectResult)purchase.GetTokens().Result)?.Value
                     as List<TourPurchaseTokenDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.Select(t => t.TourId).ShouldContain(tour1);
        result.Select(t => t.TourId).ShouldContain(tour2);
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
