using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/bundles")]
    [ApiController]
    public class BundlePurchaseController : ControllerBase
    {
        private readonly IBundlePurchaseService _bundlePurchaseService;
        private readonly IBundleService _bundleService;

        public BundlePurchaseController(
            IBundlePurchaseService bundlePurchaseService,
            IBundleService bundleService)
        {
            _bundlePurchaseService = bundlePurchaseService;
            _bundleService = bundleService;
        }

        [HttpGet]
        public IActionResult GetPublishedBundles()
        {
            try
            {
                var result = _bundleService.GetPublished();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBundleById(long id)
        {
            try
            {
                var result = _bundleService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/purchase")]
        public IActionResult PurchaseBundle(long id)
        {
            try
            {
                Console.WriteLine($"\n========== CONTROLLER: Purchase request for bundle {id} ==========");

                var touristId = GetTouristId();
                Console.WriteLine($"[CONTROLLER] Tourist ID extracted: {touristId}");

                var result = _bundlePurchaseService.PurchaseBundle(touristId, id);

                Console.WriteLine($"[CONTROLLER] Purchase result: Success={result.Success}");

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[CONTROLLER ERROR] Unauthorized: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER ERROR] Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = ex.Message, detail = ex.ToString() });
            }
        }

        private long GetTouristId()
        {
            Console.WriteLine("[GetTouristId] Extracting tourist ID from claims...");

            foreach (var claim in HttpContext.User.Claims)
            {
                Console.WriteLine($"  Claim: {claim.Type} = {claim.Value}");
            }

            var personIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "personId");
            if (personIdClaim != null && long.TryParse(personIdClaim.Value, out var personId))
            {
                Console.WriteLine($"[GetTouristId] Found personId: {personId}");
                return personId;
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            {
                Console.WriteLine($"[GetTouristId] Found userId: {userId}");
                return userId;
            }

            var idClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");
            if (idClaim != null && long.TryParse(idClaim.Value, out var id))
            {
                Console.WriteLine($"[GetTouristId] Found id: {id}");
                return id;
            }

            Console.WriteLine("[GetTouristId ERROR] No valid ID claim found!");
            throw new UnauthorizedAccessException("Tourist ID not found in token.");
        }
        [HttpGet("purchased-ids")]
        public IActionResult GetPurchasedBundleIds()
        {
            try
            {
                var touristId = GetTouristId();
                var purchasedIds = _bundlePurchaseService.GetPurchasedBundleIds(touristId);
                return Ok(purchasedIds);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}