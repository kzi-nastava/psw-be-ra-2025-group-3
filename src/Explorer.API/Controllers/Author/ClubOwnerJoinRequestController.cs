using Explorer.Stakeholders.API.Dtos; 
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/club-owner/club-join-request")]
    public class ClubOwnerJoinRequestController : Controller
    {
        private readonly IClubJoinRequestService _requestService;

        public ClubOwnerJoinRequestController(IClubJoinRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost("{id:long}/respond")]
        public ActionResult Respond(long id, [FromQuery] bool accepted)
        {
            _requestService.Respond(User.PersonId(), id, accepted);
            return Ok();
        }

        [HttpGet("{clubId:long}")]
        public ActionResult<List<ClubJoinRequestByTouristDto>> GetRequests(long clubId)
        {
            var result = _requestService.GetClubJoinRequests(User.PersonId(), clubId);
            return Ok(result);
        }
    }
}