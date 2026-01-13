using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/bundles")]
    [ApiController]
    public class BundleController : ControllerBase
    {
        private readonly IBundleService _bundleService;

        public BundleController(IBundleService bundleService)
        {
            _bundleService = bundleService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] BundleCreateDto bundleDto)
        {
            var authorId = GetAuthorId();
            var result = _bundleService.Create(bundleDto, authorId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] BundleUpdateDto bundleDto)
        {
            bundleDto.Id = id;
            var authorId = GetAuthorId();
            var result = _bundleService.Update(bundleDto, authorId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var authorId = GetAuthorId();
            _bundleService.Delete(id, authorId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _bundleService.GetById(id);
            return Ok(result);
        }

        [HttpGet("my")]
        public IActionResult GetMyBundles()
        {
            var authorId = GetAuthorId();
            var result = _bundleService.GetByAuthorId(authorId);
            return Ok(result);
        }

        [HttpPut("{id}/publish")]
        public IActionResult Publish(long id)
        {
            var authorId = GetAuthorId();
            var result = _bundleService.Publish(id, authorId);
            return Ok(result);
        }

        [HttpPut("{id}/archive")]
        public IActionResult Archive(long id)
        {
            var authorId = GetAuthorId();
            var result = _bundleService.Archive(id, authorId);
            return Ok(result);
        }

        [HttpGet("published")]
        [AllowAnonymous]
        public IActionResult GetPublished()
        {
            var result = _bundleService.GetPublished();
            return Ok(result);
        }

        private long GetAuthorId()
        {
            var personIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "personId");
            if (personIdClaim != null && long.TryParse(personIdClaim.Value, out var personId))
            {
                return personId;
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Author ID not found in token.");
        }
    }
}