using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
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
    }
}