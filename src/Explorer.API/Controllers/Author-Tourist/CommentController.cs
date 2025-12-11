using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author_Tourist
{
    [Authorize(Policy = "touristOrAuthorPolicy")]
    [Route("api/blog/{blogId}/comments")]
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
            return Ok(_service.AddComment(blogId, userId, dto.Text));
        }

        [HttpPut("{commentId}")]
        public ActionResult<CommentDto> EditComment(long blogId, long commentId, [FromBody] CommentCreateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            return Ok(_service.EditComment(blogId, commentId, userId, dto.Text));
        }

        [HttpDelete("{commentId}")]
        public IActionResult DeleteComment(long blogId, long commentId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            _service.DeleteComment(blogId, commentId, userId);
            return NoContent();
        }
    }
}
