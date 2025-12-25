using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Public
{
    [ApiController]
    [Route("api/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly NewsletterService _service;

        public NewsletterController(NewsletterService service)
        {
            _service = service;
        }

        [HttpPost("subscribe")]
        [AllowAnonymous]
        public IActionResult Subscribe([FromBody] NewsletterSubscriptionDto dto)
        {
            _service.Subscribe(dto.Email);
            return Ok();
        }
    }
}
