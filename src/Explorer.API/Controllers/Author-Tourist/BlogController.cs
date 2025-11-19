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

        [HttpPost]
        public ActionResult<BlogDto> CreateBlog([FromBody] BlogDto blogDto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            blogDto.AuthorId = userId;
            var result = _blogService.CreateBlog(blogDto);
            return CreatedAtAction(nameof(GetUserBlogs), new { userId = result.AuthorId }, result);
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
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _blogService.GetUserBlogs(userId);
            return Ok(result);
        }
    }
}