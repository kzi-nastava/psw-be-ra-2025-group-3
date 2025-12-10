using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
            return CreatedAtAction(nameof(GetBlogById), new { id = result.Id }, result);
        }

        [HttpGet("all")]
        public ActionResult<List<BlogDto>> GetAllBlogs()
        {
            var result = _blogService.GetAllBlogs();
            return Ok(result);
        }

        [HttpGet("my-blogs")]
        public ActionResult<List<BlogDto>> GetUserBlogs()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _blogService.GetUserBlogs(userId);
            return Ok(result);
        }

        // src/Explorer.API/Controllers/Author-Tourist/BlogController.cs

        [HttpGet("{id:long}")]
        public ActionResult<BlogDto> GetBlogById(long id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            // Pokušaj da nađeš blog među korisnikovim blogovima (draft može da vidi)
            var myBlogs = _blogService.GetUserBlogs(userId);
            var myBlog = myBlogs.FirstOrDefault(b => b.Id == id);

            if (myBlog != null)
            {
                // Korisnik je vlasnik, može da vidi sve statuse
                return Ok(myBlog);
            }

            // Nije vlasnik, može da vidi samo Published i Archived
            var publicBlog = _blogService.GetAllBlogs().FirstOrDefault(b => b.Id == id);

            if (publicBlog == null)
                return NotFound("Blog does not exist or is not available.");

            return Ok(publicBlog);
        }

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

            try
            {
                var result = _blogService.UpdateBlog(blogDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPatch("{id:long}/status")]
        public ActionResult<BlogDto> ChangeStatus(long id, [FromBody] int newStatus)
        {
            if (newStatus < 0 || newStatus > 2)
                return BadRequest("Status mora biti 0, 1 ili 2");

            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            try
            {
                var result = _blogService.ChangeStatus(id, userId, newStatus);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Nije tvoj blog.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}