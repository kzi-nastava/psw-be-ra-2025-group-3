using Explorer.Activity.Core.Domain;
using Explorer.Activity.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Explorer.API.Controllers.Activity
{
    [ApiController]
    [Route("api/activity")]
    [Authorize(Policy = "touristOrAuthorPolicy")]
    public class ActivityController : ControllerBase
    {
        private readonly IUserContentViewRepository _activityRepository;
        private readonly IBlogService _blogService;

        public ActivityController(
            IUserContentViewRepository activityRepository,
            IBlogService blogService)
        {
            _activityRepository = activityRepository;
            _blogService = blogService;
        }

        // =========================================================
        // 1️⃣ REGISTRACIJA PREGLEDA BLOGA
        // =========================================================
        // Poziva se kad korisnik otvori blog detalje
        // Primer: Korisnik 5 je otvorio Blog 12 u 14:30
        // =========================================================
        [HttpPost("blogs/{blogId:long}/view")]
        public IActionResult RegisterBlogView(long blogId)
        {
            var userId = GetUserId();

            var view = new UserContentView(
                userId: userId,
                contentId: blogId,
                contentType: ContentType.Blog
            );

            _activityRepository.Add(view);

            return Ok();
        }

        // =========================================================
        // 2️⃣ PERSONALIZOVANI BLOG FEED
        // =========================================================
        // Vraća blogove koje je korisnik najčešće gledao
        // i koji su mu verovatno najzanimljiviji
        // =========================================================
        [HttpGet("blogs/recommended")]
        public ActionResult<List<BlogDto>> GetRecommendedBlogs(
            [FromQuery] int take = 6)
        {
            var userId = GetUserId();

            // 1. Uzmi ID-eve blogova koje je korisnik najčešće gledao
            var mostViewedBlogIds =
                _activityRepository.GetMostViewedBlogIdsForUser(userId, take);

            if (!mostViewedBlogIds.Any())
                return Ok(new List<BlogDto>());

            // 2. Uzmi sve blogove (published / active / famous)
            var allBlogs = _blogService.GetAllBlogs();

            // 3. Filtriraj i sortiraj:
            //    - blog mora biti u mostViewed
            //    - dodatno sortiraj po score-u
            var recommendedBlogs = allBlogs
                .Where(b => mostViewedBlogIds.Contains(b.Id))
                .OrderByDescending(b => b.IsFamous)
                .ThenByDescending(b => b.IsActive)
                .ThenByDescending(b => b.CommentsCount)
                .Take(take)
                .ToList();

            return Ok(recommendedBlogs);
        }

        // =========================================================
        // 3️⃣ HELPER – USER ID IZ TOKENA
        // =========================================================
        private int GetUserId()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User id not found in token.");

            return int.Parse(userIdClaim.Value);
        }

        // =========================================================
        // ⭐ NOVO – SAMO ID-EVI (za frontend)
        // =========================================================
        [HttpGet("blogs/recommended-ids")]
        public ActionResult<List<long>> GetRecommendedBlogIds(
            [FromQuery] int take = 6)
        {
            var userId = GetUserId();

            var ids = _activityRepository
                .GetMostViewedBlogIdsForUser(userId, take);

            return Ok(ids);
        }

    }
}
