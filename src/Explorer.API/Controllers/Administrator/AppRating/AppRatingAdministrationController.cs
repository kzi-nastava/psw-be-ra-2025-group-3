using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.AppRating
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/app-rating")]
    [ApiController]
    public class AppRatingAdministrationController : ControllerBase
    {
        private readonly IAppRatingService _appRatingService;

        public AppRatingAdministrationController(IAppRatingService appRatingService)
        {
            _appRatingService = appRatingService;
        }

        [HttpGet]
        public ActionResult<List<AppRatingResponseDto>> GetAll()
        {
            var result = _appRatingService.GetAllRatings();
            return Ok(result);
        }
    }
}
