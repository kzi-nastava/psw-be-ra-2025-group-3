using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Shopping
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public ActionResult<ShoppingCartDto> Get()
        {
            return Ok(_shoppingCartService.GetMyCart(User.PersonId()));
        }

        [HttpPost("items")]
        public ActionResult<ShoppingCartDto> Add([FromBody] ShoppingCartRequestDto request)
        {
            return Ok(_shoppingCartService.AddToCart(User.PersonId(), request.TourId));
        }

        [HttpDelete("items/{tourId:long}")]
        public ActionResult<ShoppingCartDto> Remove(long tourId)
        {
            return Ok(_shoppingCartService.RemoveFromCart(User.PersonId(), tourId));
        }


        [HttpPost("add-bundle/{bundleId}")]
        public IActionResult AddBundleToCart(long bundleId)
        {
            var result = _shoppingCartService.AddBundleToCart(User.PersonId(), bundleId);
            return Ok(result);
        }
        [HttpDelete("bundles/{bundleId:long}")]
        public ActionResult<ShoppingCartDto> RemoveBundle(long bundleId)
        {
            return Ok(_shoppingCartService.RemoveBundleFromCart(User.PersonId(), bundleId));
        }
        [HttpGet("has-purchased-bundle/{bundleId}")]
        public ActionResult<bool> HasPurchasedBundle(long bundleId)
        {
            return Ok(_shoppingCartService.HasPurchasedBundle(User.PersonId(), bundleId));
        }
    }
}