using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
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

        public TouristToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<List<TourDto>> GetPublishedTours()
        {
            var tours = _tourService.GetPublished();
            return Ok(tours);
        }
    }
}
