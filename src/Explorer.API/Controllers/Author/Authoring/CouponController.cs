using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/coupons")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpPost]
    public ActionResult<CouponDto> Create([FromBody] CouponCreateDto couponDto)
    {
        var authorId = GetAuthorId();
        var result = _couponService.Create(couponDto, authorId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public ActionResult<List<CouponDto>> GetMyCoupons()
    {
        var authorId = GetAuthorId();
        var result = _couponService.GetByAuthorId(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<CouponDto> GetById(long id)
    {
        var authorId = GetAuthorId();
        var result = _couponService.GetById(id, authorId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<CouponDto> Update(long id, [FromBody] CouponUpdateDto couponDto)
    {
        var authorId = GetAuthorId();
        var result = _couponService.Update(id, couponDto, authorId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = GetAuthorId();
        _couponService.Delete(id, authorId);
        return NoContent();
    }

    private long GetAuthorId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out var authorId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
        }
        return authorId;
    }
}
