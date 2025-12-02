// Explorer.API/Controllers/Tourist/Review/TourReviewController.cs
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Review;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-review")]
[ApiController]
public class TourReviewController : ControllerBase
{
    private readonly ITourReviewService _tourReviewService;

    public TourReviewController(ITourReviewService tourReviewService)
    {
        _tourReviewService = tourReviewService;
    }

    [HttpGet("eligibility/{tourId}")]
    public ActionResult<TourReviewEligibilityDto> CheckEligibility(long tourId)
    {
        long touristId = long.Parse(User.FindFirst("id")!.Value);
        var result = _tourReviewService.CheckEligibility(tourId, touristId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TourReviewDto> CreateReview([FromBody] TourReviewCreateDto dto)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourReviewService.CreateReview(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public ActionResult<TourReviewDto> UpdateReview([FromBody] TourReviewUpdateDto dto)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourReviewService.UpdateReview(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("tour/{tourId}")]
    [AllowAnonymous] // Svi mogu da vide recenzije
    public ActionResult<List<TourReviewDto>> GetReviewsForTour(long tourId)
    {
        var result = _tourReviewService.GetReviewsForTour(tourId);
        return Ok(result);
    }

    [HttpGet("my-review/{tourId}")]
    public ActionResult<TourReviewDto> GetMyReview(long tourId)
    {
        long touristId = long.Parse(User.FindFirst("id")!.Value);
        var result = _tourReviewService.GetMyReview(tourId, touristId);

        if (result == null)
            return NotFound(new { message = "You haven't reviewed this tour yet." });

        return Ok(result);
    }
}