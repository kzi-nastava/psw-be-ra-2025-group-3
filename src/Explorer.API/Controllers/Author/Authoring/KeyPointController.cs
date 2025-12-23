using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [AllowAnonymous]
        public ActionResult<PagedResult<KeyPointDto>> GetPaged(
            [FromQuery] long tourId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = _keyPointService.GetPaged(tourId, page, pageSize);
            return Ok(result);
        }

        // GET api/keypoints/5
        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public ActionResult<KeyPointDto> GetById(long id)
        {
            var result = _keyPointService.GetById(id);
            return Ok(result);
        }

        // POST api/keypoints
        [HttpPost]
        public ActionResult<KeyPointDto> Create([FromBody] KeyPointDto dto)
        {
            var authorId = GetAuthorId();
            var result = _keyPointService.Create(dto, authorId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/keypoints/5
        [HttpPut("{id:long}")]
        public ActionResult<KeyPointDto> Update(long id, [FromBody] KeyPointDto dto)
        {
            var existing = _keyPointService.GetById(id);

            if (!string.IsNullOrWhiteSpace(existing.ImageUrl) &&
                existing.ImageUrl != dto.ImageUrl)
            {
                var fileName = Path.GetFileName(existing.ImageUrl);
                var filePath = Path.Combine(
                    "wwwroot",
                    "uploads",
                    "keypoint-images",
                    fileName
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            dto.Id = id;
            var authorId = GetAuthorId();
            var result = _keyPointService.Update(dto, authorId);
            return Ok(result);
        }

        // DELETE api/keypoints/5
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            var kp = _keyPointService.GetById(id);

            if (!string.IsNullOrWhiteSpace(kp.ImageUrl))
            {
                var fileName = Path.GetFileName(kp.ImageUrl);
                var filePath = Path.Combine(
                    "wwwroot",
                    "uploads",
                    "keypoint-images",
                    fileName
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            var authorId = GetAuthorId();
            _keyPointService.Delete(id, authorId);
            return NoContent();
        }

        // === IDENTIČNO TourController helper metodi ===
        private long GetAuthorId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);

            if (claim == null || !long.TryParse(claim.Value, out var authorId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
            }

            return authorId;
        }
    }
}
