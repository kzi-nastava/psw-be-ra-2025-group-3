using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tours")]
    [ApiController]
    public class TouristToursController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ITouristTourService _touristTourService;
        private readonly ITourExecutionService _executionService;

        public TouristToursController(
            ITourService tourService,
            ITouristTourService touristTourService, ITourExecutionService executionService)
        {
            _tourService = tourService;
            _touristTourService = touristTourService;
            _executionService = executionService;
        }


        [HttpGet]
        public ActionResult<List<TourDto>> GetPublishedTours()
        {
            try
            {
                var tours = _tourService.GetPublished();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load published tours.", error = ex.Message });
            }
        }

        //Istaknute ture za neprijavljene korisnike
        [HttpGet("highlighted")]
        [AllowAnonymous]
        public ActionResult<List<TourPreviewDto>> GetHighlightedTours([FromQuery] int count = 6)
        {
            try
            {
                var result = _touristTourService.GetPublishedTours()
                    .OrderByDescending(t => t.AverageRating)
                    .ThenByDescending(t => t.Reviews.Count)
                    .Take(count)
                    .ToList();
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load highlighted tours.", error = ex.Message });
            }
        }

        [HttpGet("{id}/preview")]
        [AllowAnonymous] // Dodato da neprijavljeni mogu videti preview
        public ActionResult<TourPreviewDto> GetTourPreview(long id)
        {
            var result = _touristTourService.GetPreview(id);
            return Ok(result);
        }


        [HttpGet("{id}/details")]
        public ActionResult<TourDetailsDto> GetTourDetails(long id)
        {
            long touristId = GetTouristId();
            var result = _touristTourService.GetDetails(touristId, id);
            return Ok(result);
        }
        //tour execution
        [HttpGet("my-tours")]
        public ActionResult<List<TourPreviewDto>> GetMyPurchasedTours()
        {
            long touristId = GetTouristId();
            var result = _touristTourService.GetMyPurchasedTours(touristId);
            return Ok(result);
        }

        [HttpGet("{id}/can-start")]
        public ActionResult CanStart(long id)
        {
            long touristId = GetTouristId();

            bool canStart = _executionService.CanStartTour(touristId, id);

            if (!canStart)
                return Forbid("Tour not purchased.");

            return Ok(new { message = "OK" });
        }

        // Endpoint za pretragu i filtriranje
        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<List<TourPreviewDto>> SearchTours(
            [FromQuery] string? name,
            [FromQuery] List<string>? tags,
            [FromQuery] List<int>? difficulties,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] double? minRating,
            [FromQuery] bool? onSale,
            [FromQuery] bool? sortByDiscount)
        {
            try
            {
                var filters = new TourFilterDto { Name = name, Tags = tags, Difficulties = difficulties, MinPrice = minPrice, MaxPrice = maxPrice, MinRating = minRating, OnSale = onSale, SortByDiscount = sortByDiscount };
                var result = _touristTourService.SearchAndFilterTours(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to search tours.", error = ex.Message });
            }
        }

        private long GetTouristId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
            return long.Parse(claim.Value);
        }
    }
}
