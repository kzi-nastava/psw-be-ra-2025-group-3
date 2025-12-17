// Explorer.API/Controllers/Tourist/Review/TourReviewController.cs
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.IO;
using System;

namespace Explorer.API.Controllers.Tourist.Review;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-review")]
[ApiController]
public class TourReviewController : ControllerBase
{
    private readonly ITourReviewService _tourReviewService;
    private readonly IPersonRepository _personRepository;

    public TourReviewController(ITourReviewService tourReviewService, IPersonRepository personRepository)
    {
        _tourReviewService = tourReviewService;
        _personRepository = personRepository;
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
    [AllowAnonymous]
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


    [HttpGet("my-reviews")]
    public ActionResult<List<TourReviewDto>> GetMyAllReviews()
    {
        long touristId = long.Parse(User.FindFirst("id")!.Value);
        var result = _tourReviewService.GetAllReviewsForTourist(touristId);
        return Ok(result);
    }

    [HttpPost("{reviewId}/images")]
    public ActionResult<ReviewImageDto> AddImageToReview(long reviewId, [FromBody] AddImageRequest request)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _tourReviewService.AddImageToReview(reviewId, touristId, request.ImageUrl);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{reviewId}/images/{imageId}")]
    public ActionResult DeleteImageFromReview(long reviewId, long imageId)
    {
        try
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);

            var image = _tourReviewService.GetImageById(reviewId, imageId);
            if (image != null)
            {
                var fileName = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine("wwwroot", "uploads", "review-images", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _tourReviewService.DeleteImageFromReview(reviewId, imageId, touristId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("tourist-name/{touristId}")]
    [AllowAnonymous]
    public ActionResult<string> GetTouristName(long touristId)
    {
        var person = _personRepository.GetByUserId(touristId);

        if (person == null)
            return Ok("Anonymous");

        return Ok($"{person.Name} {person.Surname}");
    }
}


public class AddImageRequest
{
    public string ImageUrl { get; set; } = string.Empty; // ✅ DODAJ default vrednost
}