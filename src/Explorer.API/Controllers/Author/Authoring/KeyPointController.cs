using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/keypoints")]
    [ApiController]
    public class KeyPointController : ControllerBase
    {
        private readonly IKeyPointService _keyPointService;

        public KeyPointController(IKeyPointService keyPointService)
        {
            _keyPointService = keyPointService;
        }

        // GET api/keypoints?page=0&pageSize=10
        [HttpGet]
        public ActionResult<PagedResult<KeyPointDto>> GetPaged(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = _keyPointService.GetPaged(page, pageSize);
            return Ok(result);
        }

        // GET api/keypoints/5
        [HttpGet("{id:long}")]
        public ActionResult<KeyPointDto> GetById(long id)
        {
            var result = _keyPointService.GetById(id);
            return Ok(result);
        }

        // POST api/keypoints
        [HttpPost]
        public ActionResult<KeyPointDto> Create([FromBody] KeyPointDto dto)
        {
            var result = _keyPointService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/keypoints/5
        [HttpPut("{id:long}")]
        public ActionResult<KeyPointDto> Update(long id, [FromBody] KeyPointDto dto)
        {
            dto.Id = id;
            var result = _keyPointService.Update(dto);
            return Ok(result);
        }

        // DELETE api/keypoints/5
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _keyPointService.Delete(id);
            return NoContent();
        }
    }
}
