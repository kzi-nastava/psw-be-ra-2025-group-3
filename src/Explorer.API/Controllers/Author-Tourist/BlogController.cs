using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // -----------------------------
        // CREATE
        // -----------------------------
        [HttpPost]
        public ActionResult<BlogDto> CreateBlog([FromBody] BlogDto blogDto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            blogDto.AuthorId = userId;

            var result = _blogService.CreateBlog(blogDto);
            return CreatedAtAction(nameof(GetBlogById), new { id = result.Id }, result);
        }

        // -----------------------------
        // GET ALL (PUBLIC)
        // -----------------------------
        [HttpGet("all")]
        public ActionResult<List<BlogDto>> GetAllBlogs()
        {
            var result = _blogService.GetAllBlogs();
            return Ok(result);
        }

        // -----------------------------
        // GET MY BLOGS (FILTER IRREPLACEABLE)
        // -----------------------------
        [HttpGet("my-blogs")]
        public ActionResult<List<BlogDto>> GetUserBlogs()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _blogService.GetUserBlogs(userId);
            return Ok(result);
        }

        // -----------------------------
        // GET BY ID (ONLY IF OWNER)
        // -----------------------------
        [HttpGet("{id:long}")]
        public ActionResult<BlogDto> GetBlogById(long id)
        {
            var blog = _blogService.GetAllBlogs().FirstOrDefault(b => b.Id == id);

            if (blog == null)
                return NotFound("Blog ne postoji.");

            return Ok(blog);
        }

        // -----------------------------
        // UPDATE (ONLY IF OWNER)
        // -----------------------------
        [HttpPut("{id:long}")]
        public ActionResult<BlogDto> UpdateBlog(long id, [FromBody] BlogDto blogDto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            var myBlogs = _blogService.GetUserBlogs(userId);
            var existing = myBlogs.FirstOrDefault(b => b.Id == id);

            if (existing == null)
                return Forbid("Nije tvoj blog.");

            blogDto.Id = id;
            blogDto.AuthorId = userId;

            var result = _blogService.UpdateBlog(blogDto);
            return Ok(result);
        }
    }
}
