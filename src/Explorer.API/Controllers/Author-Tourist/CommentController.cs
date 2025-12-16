using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Explorer.API.Controllers.Author_Tourist
{
    [Authorize(Policy = "touristOrAuthorPolicy")]
    [Route("api/blog/{blogId:long}/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IBlogService _service;

        public CommentController(IBlogService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<CommentDto> AddComment(long blogId, [FromBody] CommentCreateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            try
            {
                return Ok(_service.AddComment(blogId, userId, dto.Text));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{commentId:long}")]
        public ActionResult<CommentDto> EditComment(long blogId, long commentId, [FromBody] CommentCreateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            try
            {
                return Ok(_service.EditComment(blogId, commentId, userId, dto.Text));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{commentId:long}")]
        public IActionResult DeleteComment(long blogId, long commentId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            try
            {
                _service.DeleteComment(blogId, commentId, userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
