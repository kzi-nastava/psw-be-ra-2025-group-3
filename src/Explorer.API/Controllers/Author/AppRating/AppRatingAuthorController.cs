using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.AppRating
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/app-rating")]
    [ApiController]
    public class AppRatingAuthorController : ControllerBase
    {
        private readonly IAppRatingService _appRatingService;

        public AppRatingAuthorController(IAppRatingService appRatingService)
        {
            _appRatingService = appRatingService;
        }

        [HttpGet]
        public ActionResult<AppRatingResponseDto?> GetMyRating()
        {
            var result = _appRatingService.GetMyRating(User.PersonId());
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<AppRatingResponseDto> Create([FromBody] AppRatingRequestDto request)
        {
            var result = _appRatingService.CreateRating(User.PersonId(), request);
            return Ok(result);
        }

        [HttpPut]
        public ActionResult<AppRatingResponseDto> Update([FromBody] AppRatingRequestDto request)
        {
            var result = _appRatingService.UpdateRating(User.PersonId(), request);
            return Ok(result);
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            _appRatingService.DeleteRating(User.PersonId());
            return Ok();
        }
    }
}
