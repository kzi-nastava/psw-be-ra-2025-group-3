// src/Explorer.API/Controllers/Shopping/TourPurchaseController.cs
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Shopping
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

        // src/Explorer.API/Controllers/Shopping/TourPurchaseController.cs
        [HttpPost("checkout")]
        public ActionResult<CheckoutResultDto> Checkout()
        {
            try
            {
                Console.WriteLine("=== CHECKOUT CONTROLLER START ===");
                var personId = GetPersonIdFromToken();
                Console.WriteLine($"Person ID: {personId}");

                var result = _purchaseService.Checkout(personId);
                Console.WriteLine($"Service returned: Success={result.Success}");

                if (!result.Success)
                {
                    Console.WriteLine($"BadRequest: {result.Message}");
                    return BadRequest(result);
                }

                Console.WriteLine($"OK: {result.Tokens.Count} tokens");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message });
            }
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