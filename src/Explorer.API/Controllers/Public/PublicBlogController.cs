using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Public
{
    [ApiController]
    [Route("api/blog")]
    public class PublicBlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public PublicBlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet("public")]
        public ActionResult<List<BlogDto>> GetPublicBlogs()
        {
            var result = _blogService.GetAllBlogs();
            return Ok(result);
        }
    }
}
