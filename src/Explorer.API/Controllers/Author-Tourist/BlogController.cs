using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author_Tourist
{
    [Authorize(Policy = "touristOrAuthorPolicy")]
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }

        [HttpPost]
        public ActionResult<BlogDto> CreateBlog([FromBody] BlogDto blogDto)
        {
            // ✅ Uzmi userId iz JWT tokena
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            blogDto.AuthorId = userId;

            var result = _blogService.CreateBlog(blogDto);
            return CreatedAtAction(nameof(GetBlogById), new { id = result.Id }, result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<BlogDto> UpdateBlog(long id, [FromBody] BlogDto blogDto)
        {
            // ✅ Uzmi userId iz JWT tokena
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            // ✅ Proveri da li korisnik sme da menja ovaj blog
            var existingBlogs = _blogService.GetUserBlogs(userId);
            var existingBlog = existingBlogs.FirstOrDefault(b => b.Id == id);

            if (existingBlog == null)
            {
                return Forbid(); // 403 - Nije tvoj blog!
            }

            // ✅ Postavi ID i AuthorId da spreči manipulaciju
            blogDto.Id = id;
            blogDto.AuthorId = userId;

            var result = _blogService.UpdateBlog(blogDto);
            return Ok(result);
        }

        [HttpGet("my-blogs")]
        public ActionResult<List<BlogDto>> GetUserBlogs()
        {
            // ✅ Uzmi userId iz JWT tokena
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            var result = _blogService.GetUserBlogs(userId);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<BlogDto> GetBlogById(long id)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
                var blogs = _blogService.GetUserBlogs(userId);
                var blog = blogs.FirstOrDefault(b => b.Id == id);

                if (blog == null)
                {
                    return NotFound($"Blog sa ID {id} nije pronađen ili nije tvoj.");
                }

                return Ok(blog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/blog/{id}/vote
        [HttpPost("{id:long}/vote")]
        public ActionResult<BlogVoteStateDto> Vote (long id, [FromBody] BlogVoteDto dto)
        {
            var userId = GetUserId();
            if (dto.BlogId != 0 && dto.BlogId != id)
                return BadRequest("BlogId in body does not match route id.");

            var result = _blogService.Vote(id, userId, dto.IsUpvote);

            return Ok(result);
        }

        // GET api/blog/{id}/vote
        [HttpGet("{id:long}/vote")]
        public ActionResult<BlogVoteStateDto> GetVoteState(long id)
        {
            var userId = GetUserId();
            var result = _blogService.GetUserVoteState(id, userId);
            return Ok(result);
        }
    }
}