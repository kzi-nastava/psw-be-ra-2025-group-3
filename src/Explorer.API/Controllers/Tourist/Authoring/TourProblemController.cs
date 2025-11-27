using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist.Authoring;

[Authorize(Policy = "touristPolicy")]
[Route("api/tour-problems")]
public class TourProblemController : ControllerBase
{
    private readonly ITourProblemService _tourProblemService;
    private readonly ITourRepository _tourRepository; // DODAJ OVO

    // IZMENI KONSTRUKTOR
    public TourProblemController(ITourProblemService tourProblemService, ITourRepository tourRepository)
    {
        _tourProblemService = tourProblemService;
        _tourRepository = tourRepository; // DODAJ OVO
    }

    [HttpPost]
    public ActionResult<TourProblemDto> Create([FromBody] TourProblemCreateDto problemDto)
    {
        var touristId = GetTouristId();
        var result = _tourProblemService.Create(problemDto, touristId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    public ActionResult<List<TourProblemDto>> GetMyProblems()
    {
        var touristId = GetTouristId();
        var result = _tourProblemService.GetByTouristId(touristId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<TourProblemDto> GetById(long id)
    {
        var touristId = GetTouristId();
        var result = _tourProblemService.GetById(id, touristId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<TourProblemDto> Update(long id, [FromBody] TourProblemUpdateDto problemDto)
    {
        problemDto.Id = id;
        var touristId = GetTouristId();
        var result = _tourProblemService.Update(problemDto, touristId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var touristId = GetTouristId();
        _tourProblemService.Delete(id, touristId);
        return NoContent();
    }

  
    [HttpGet("validate-tour/{tourId}")]
    public IActionResult ValidateTour(long tourId)
    {
        try
        {
            var tour = _tourRepository.GetById(tourId);
            return Ok(new { exists = tour != null });
        }
        catch
        {
            return Ok(new { exists = false });
        }
    }

    // Helper metoda za ekstrakciju Tourist ID iz JWT tokena
    private long GetTouristId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out var touristId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
        }
        return touristId;
    }
}