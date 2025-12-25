using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/meetups")]
[ApiController]
public class TouristMeetupController : ControllerBase
{
    private readonly IMeetupService _meetupService;

    public TouristMeetupController(IMeetupService meetupService)
    {
        _meetupService = meetupService;
    }

    [HttpGet]
    public ActionResult<List<MeetupDto>> GetAll()
    {
        var result = _meetupService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<MeetupDto> GetById(long id)
    {
        var result = _meetupService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<MeetupDto> Create([FromBody] MeetupCreateDto meetupDto)
    {
        var creatorId = User.PersonId();
        var result = _meetupService.Create(meetupDto, creatorId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<MeetupDto> Update(long id, [FromBody] MeetupUpdateDto meetupDto)
    {
        var creatorId = User.PersonId();
        var result = _meetupService.Update(id, meetupDto, creatorId);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var creatorId = User.PersonId();
        _meetupService.Delete(id, creatorId);
        return Ok();
    }
}
