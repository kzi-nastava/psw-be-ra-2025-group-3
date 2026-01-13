using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Shopping;

[Collection("Sequential")]
public class BundlePurchaseCommandTests : BasePaymentsIntegrationTest
{
    public BundlePurchaseCommandTests(PaymentsTestFactory factory) : base(factory) { }

    private static string NewPersonId() => (-10000 - Guid.NewGuid().GetHashCode()).ToString();

    [Fact]
    public void PurchaseBundle_creates_payment_record_and_tokens()
    {
        // Arrange
        var personId = NewPersonId();
        var bundleId = -1; // Published bundle sa 2 ture [-2, -4]

        using var scope = Factory.Services.CreateScope();
        var controller = CreateBundlePurchaseController(scope, personId);
        var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Act
        var actionResult = controller.PurchaseBundle(bundleId);
        var okResult = actionResult as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var result = okResult.Value as BundlePurchaseResultDto;
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Successfully purchased");

        // Proveri payment record
        result.PurchaseRecord.ShouldNotBeNull();
        result.PurchaseRecord.BundleId.ShouldBe(bundleId);
        result.PurchaseRecord.TouristId.ShouldBe(long.Parse(personId));
        result.PurchaseRecord.PriceAc.ShouldBe(1000m);

        // Proveri tokene (2 ture u bundle-u)
        result.Tokens.Count.ShouldBe(2);
        result.Tokens.Select(t => t.TourId).ShouldContain(-2L);
        result.Tokens.Select(t => t.TourId).ShouldContain(-4L);

        // Proveri da su tokeni u bazi
        var tokensInDb = db.TourPurchaseTokens
            .Where(t => t.TouristId == long.Parse(personId))
            .ToList();
        tokensInDb.Count.ShouldBe(2);
    }

    [Fact]
    public void PurchaseBundle_fails_if_bundle_not_found()
    {
        // Arrange
        var personId = NewPersonId();
        var bundleId = -99; // Ne postoji

        using var scope = Factory.Services.CreateScope();
        var controller = CreateBundlePurchaseController(scope, personId);

        // Act
        var actionResult = controller.PurchaseBundle(bundleId);
        var badRequestResult = actionResult as BadRequestObjectResult;

        // Assert
        badRequestResult.ShouldNotBeNull();
        var result = badRequestResult.Value as BundlePurchaseResultDto;
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("not found");
    }

    [Fact]
    public void PurchaseBundle_fails_if_bundle_not_published()
    {
        // Arrange
        var personId = NewPersonId();
        var bundleId = -2; // Draft status

        using var scope = Factory.Services.CreateScope();
        var controller = CreateBundlePurchaseController(scope, personId);

        // Act
        var actionResult = controller.PurchaseBundle(bundleId);
        var badRequestResult = actionResult as BadRequestObjectResult;

        // Assert
        badRequestResult.ShouldNotBeNull();
        var result = badRequestResult.Value as BundlePurchaseResultDto;
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("not available");
    }

    [Fact]
    public void PurchaseBundle_creates_bundle_purchase_record_in_database()
    {
        // Arrange
        var personId = NewPersonId();
        var bundleId = -3; // Cheap bundle, 200 AC

        using var scope = Factory.Services.CreateScope();
        var controller = CreateBundlePurchaseController(scope, personId);
        var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Act
        var actionResult = controller.PurchaseBundle(bundleId);
        var okResult = actionResult as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();

        var recordInDb = db.BundlePurchaseRecords
            .FirstOrDefault(r => r.TouristId == long.Parse(personId) && r.BundleId == bundleId);

        recordInDb.ShouldNotBeNull();
        recordInDb.PriceAc.ShouldBe(200m);
        recordInDb.BundleId.ShouldBe(bundleId);
    }

    [Fact]
    public void PurchaseBundle_generates_unique_tokens_for_each_tour()
    {
        // Arrange
        var personId = NewPersonId();
        var bundleId = -1; // Bundle sa 2 ture

        using var scope = Factory.Services.CreateScope();
        var controller = CreateBundlePurchaseController(scope, personId);

        // Act
        var actionResult = controller.PurchaseBundle(bundleId);
        var okResult = actionResult as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var result = okResult.Value as BundlePurchaseResultDto;
        result.ShouldNotBeNull();

        // Proveri da svaki token ima unique GUID
        result.Tokens.Count.ShouldBe(2);
        result.Tokens[0].Token.ShouldNotBe(result.Tokens[1].Token);

        // Proveri da su oba tokena validni GUID-ovi
        Guid.TryParse(result.Tokens[0].Token, out _).ShouldBeTrue();
        Guid.TryParse(result.Tokens[1].Token, out _).ShouldBeTrue();
    }

    private static BundlePurchaseController CreateBundlePurchaseController(IServiceScope scope, string personId)
    {
        return new BundlePurchaseController(
            scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>(),
            scope.ServiceProvider.GetRequiredService<IBundleService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}