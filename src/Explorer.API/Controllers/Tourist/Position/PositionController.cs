using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Position
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/position")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        // GET api/tourist/position
        [HttpGet]
        public ActionResult<PositionDto?> GetMyPosition()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var position = _positionService.GetForTourist(touristId);
            return Ok(position); // null će se serializovati
        }

        // PUT api/tourist/position
        [HttpPut]
        public IActionResult Update([FromBody] PositionDto dto)
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            _positionService.Update(touristId, dto);
            return Ok();
        }
    }
}
