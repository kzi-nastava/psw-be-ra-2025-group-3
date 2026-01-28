using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Execution;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-execution")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _tourExecutionService;

    public TourExecutionController(ITourExecutionService tourExecutionService)
    {
        _tourExecutionService = tourExecutionService;
    }

    [HttpPost("start")]
    public ActionResult<TourExecutionDto> StartTour([FromBody] TourExecutionCreateDto dto)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourExecutionService.StartTour(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("active")]
    public ActionResult<TourExecutionDto> GetActiveTourExecution()
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);

            var activeTourExecution = _tourExecutionService.GetActiveTourExecution(touristId);

            if (activeTourExecution == null)
                return Ok(null);

            return Ok(activeTourExecution);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load active tour execution.", error = ex.Message });
        }
    }

    [HttpGet("active/{touristId:long}")]
    public ActionResult<TourDto> GetActiveTourByTouristId(long touristId)
    {
        var activeTour = _tourExecutionService.GetActiveTourByTouristId(touristId);
        if (activeTour == null)
            return Ok(null);

        return Ok(activeTour);
    }

    [HttpGet("active-with-next-keypoint")]
    public ActionResult<TourExecutionWithNextKeyPointDto> GetActiveWithNextKeyPoint()
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourExecutionService.GetActiveWithNextKeyPoint(touristId);

            if (result == null)
                return Ok(null);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to load active tour with next key point.", error = ex.Message });
        }
    }

    [HttpPost("complete")]
    public ActionResult<TourExecutionDto> CompleteTour()
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourExecutionService.CompleteTour(touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Action failed. Please check your connection and try again." });
        }
    }

    [HttpPost("abandon")]
    public ActionResult<TourExecutionDto> AbandonTour()
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourExecutionService.AbandonTour(touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Action failed. Please check your connection and try again." });
        }
    }

    //task2
    [HttpPost("check-location")]
    public ActionResult<LocationCheckResultDto> CheckLocation([FromBody] LocationCheckDto dto)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourExecutionService.CheckLocationProgress(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}