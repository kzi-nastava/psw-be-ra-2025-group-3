using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/club-join-request")]
    public class ClubJoinRequestController : Controller 
    {
        private readonly IClubJoinRequestService _requestService;

        public ClubJoinRequestController(IClubJoinRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public ActionResult<ClubJoinRequestDto> Send([FromBody] ClubJoinRequestDto dto)
        {
            var result = _requestService.Send(User.PersonId(), dto.ClubId);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Withdraw(long id)
        {
            _requestService.Withdraw(User.PersonId(), id);
            return Ok();
        }
    }
}