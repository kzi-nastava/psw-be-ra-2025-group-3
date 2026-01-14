using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/coupons")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpPost("validate")]
    public ActionResult<CouponValidationResultDto> ValidateCoupon([FromBody] CouponValidationDto dto)
    {
        // Ako ima TourIds, validiraj za celu korpu
        if (dto.TourIds != null && dto.TourIds.Count > 0)
        {
            var result = _couponService.ValidateCouponForCart(dto.Code, dto.TourIds);
            return Ok(result);
        }
        
        // Inače, validiraj za jednu turu (backward compatibility)
        var singleResult = _couponService.ValidateCoupon(dto.Code, dto.TourId);
        return Ok(singleResult);
    }
}
