using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/purchase")]
    [ApiController]
    public class TourPurchaseController : ControllerBase
    {
        private readonly ITourPurchaseTokenService _purchaseService;

        public TourPurchaseController(ITourPurchaseTokenService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // POST: api/tourist/purchase/checkout
        [HttpPost("checkout")]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            var personId = GetPersonIdFromToken();
            var result = _purchaseService.Checkout(personId);
            return Ok(result);
        }

        // GET: api/tourist/purchase
        [HttpGet]
        public ActionResult<List<TourPurchaseTokenDto>> GetTokens()
        {
            try
            {
                var personId = GetPersonIdFromToken();
                var result = _purchaseService.GetTokens(personId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error: " + ex.Message });
            }
        }

        // Helper metoda – ista logika kao u PersonController-u
        private long GetPersonIdFromToken()
        {
            // Primarni claim
            var personIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "personId");

            if (personIdClaim != null &&
                long.TryParse(personIdClaim.Value, out var personId))
            {
                return personId;
            }

            // Sekundarni (za testove)
            var userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null &&
                long.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Person ID not found in token.");
        }
    }
}
