using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author_Tourist
{
    // PRIVREMENO UKLONJENO: [Authorize(Policy = "touristOrAuthorPolicy")]
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        public ActionResult<BlogDto> CreateBlog([FromBody] BlogDto blogDto)
        {
            blogDto.AuthorId = 2; // Hardkodirano za testiranje
            var result = _blogService.CreateBlog(blogDto);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<BlogDto> UpdateBlog(long id, [FromBody] BlogDto blogDto)
        {
            blogDto.Id = id;
            var result = _blogService.UpdateBlog(blogDto);
            return Ok(result);
        }

        [HttpGet("my-blogs")]
        public ActionResult<List<BlogDto>> GetUserBlogs()
        {
            var userId = 2; // Hardkodirano za testiranje
            var result = _blogService.GetUserBlogs(userId);
            return Ok(result);
        }
    }
}