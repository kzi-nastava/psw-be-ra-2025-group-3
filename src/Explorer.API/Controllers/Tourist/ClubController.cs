using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;


[Authorize(Policy = "touristPolicy")]
[Route("api/clubs")]
public class ClubController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubController(IClubService clubService)
    {
        _clubService = clubService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ClubDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = _clubService.GetPaged(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ClubDto> Get(long id)
    {
        try
        {
            var result = _clubService.Get(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("my-clubs")]
    public ActionResult<PagedResult<ClubDto>> GetMyClubs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var result = _clubService.GetUserClubs(userId, page, pageSize);
        return Ok(result);
    }

    private long GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    [HttpPost]
    public ActionResult<ClubDto> Create([FromBody] ClubCreateDto club)
    {
        try
        {
            var userId = GetUserId();
            var result = _clubService.Create(club, userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:long}")]
    public ActionResult<ClubDto> Update(long id, [FromBody] ClubUpdateDto club)
    {
        try
        {
            var userId = GetUserId();
            var result = _clubService.Update(id, club, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        try
        {
            var userId = GetUserId();
            _clubService.Delete(id, userId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    [HttpPut("{id:long}/status")]
    public ActionResult<ClubDto> ChangeStatus(long id, [FromBody] string status)
    {
        var userId = GetUserId();
        var result = _clubService.ChangeStatus(id, status, userId);
        return Ok(result);
    }

    [HttpPost("{id:long}/invite/{touristId:long}")]
    public ActionResult<ClubDto> InviteMember(long id, long touristId)
    {
        var userId = GetUserId();
        var result = _clubService.InviteMember(id, touristId, userId);
        return Ok(result);
    }

    [HttpDelete("{id:long}/kick/{memberId:long}")]
    public ActionResult<ClubDto> KickMember(long id, long memberId)
    {
        var userId = GetUserId();
        var result = _clubService.KickMember(id, memberId, userId);
        return Ok(result);
    }
}
