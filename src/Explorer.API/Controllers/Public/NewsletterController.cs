using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Explorer.API.Controllers.Public
{
    [ApiController]
    [Route("api/blog/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterService _newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        /// <summary>
        /// Subscribe to newsletter (public, no authentication required)
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Subscribe([FromBody] NewsletterSubscriptionDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            try
            {
                _newsletterService.Subscribe(dto.Email);
                return Ok(new { message = "Subscribed successfully." });
            }
            catch (ArgumentException ex)
            {
                // invalid email format
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // email already subscribed
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
