using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/tours")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourCreateDto tourDto)
    {
        var authorId = GetAuthorId();
        var result = _tourService.Create(tourDto, authorId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    public ActionResult<List<TourDto>> GetMyTours()
    {
        var authorId = GetAuthorId();
        var result = _tourService.GetByAuthorId(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<TourDto> GetById(long id)
    {
        var result = _tourService.GetById(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<TourDto> Update(long id, [FromBody] TourUpdateDto tourDto)
    {
        tourDto.Id = id;
        var authorId = GetAuthorId();
        var result = _tourService.Update(tourDto, authorId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = GetAuthorId();
        _tourService.Delete(id, authorId);
        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    public ActionResult<TourDto> Publish(long id)
    {
        var authorId = GetAuthorId();
        var result = _tourService.Publish(id, authorId);
        return Ok(result);
    }

    [HttpPut("{tourId}/equipment/{equipmentId}")]
    public ActionResult<TourDto> AddEquipment(long tourId, long equipmentId)
    {
        var authorId = GetAuthorId();
        var result = _tourService.AddEquipment(tourId, equipmentId, authorId);
        return Ok(result);
    }

    [HttpDelete("{tourId}/equipment/{equipmentId}")]
    public ActionResult<TourDto> RemoveEquipment(long tourId, long equipmentId)
    {
        var authorId = GetAuthorId();
        var result = _tourService.RemoveEquipment(tourId, equipmentId, authorId);
        return Ok(result);
    }

    // Helper metoda za ekstrakciju Author ID iz JWT tokena
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