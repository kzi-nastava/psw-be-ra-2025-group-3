using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/award-event")]
    public class AwardEventController : ControllerBase
    {
        private readonly IAwardEventService _awardEventService;

        public AwardEventController(IAwardEventService awardEventService)
        {
            _awardEventService = awardEventService;
        }

        [HttpGet]
        public ActionResult<PagedResult<AwardEventDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _awardEventService.GetPaged(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<AwardEventDto> Get(long id)
        {
            var result = _awardEventService.Get(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<AwardEventDto> Create([FromBody] AwardEventCreateDto createDto)
        {
            var result = _awardEventService.Create(createDto);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<AwardEventDto> Update(long id, [FromBody] AwardEventUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in URL and body do not match.");

            var result = _awardEventService.Update(updateDto);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _awardEventService.Delete(id);
            return Ok();
        }
    }
}