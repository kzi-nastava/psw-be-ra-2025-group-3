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
            var tours = _tourService.GetPublished();
            return Ok(tours);
        }


        [HttpGet("{id}/preview")]
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
        [HttpGet("{id}/can-start")]
        public ActionResult CanStart(long id)
        {
            long touristId = GetTouristId();

            bool canStart = _executionService.CanStartTour(touristId, id);

            if (!canStart)
                return Forbid("Tour not purchased.");

            return Ok(new { message = "OK" });
        }

        private long GetTouristId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
            return long.Parse(claim.Value);
        }
    }
}
