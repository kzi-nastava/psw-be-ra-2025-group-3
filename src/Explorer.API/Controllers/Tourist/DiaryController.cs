using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/diaries")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly IDiaryService _diaryService;

        public DiaryController(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        [HttpGet]
        public ActionResult<List<DiaryDto>> GetMyDiaries()
        {
            var userId = GetUserId();
            return Ok(_diaryService.GetMyDiaries(userId));
        }
        [HttpPost]
        public ActionResult<DiaryDto> Create([FromBody] DiaryCreateDto dto)
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
            }

            var userId = GetUserId();
            var diary = _diaryService.Create(dto, userId);
            return Ok(diary);
        }

        [HttpPut("{id:long}")]
        public ActionResult<DiaryDto> Update(long id, [FromBody] DiaryCreateDto dto)
        {
            var userId = GetUserId();
            var diary = _diaryService.Update(id, dto, userId);
            return Ok(diary);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            var userId = GetUserId();
            _diaryService.Delete(id, userId);
            return NoContent();
        }

        [HttpPost("{id:long}/archive")]
        public ActionResult<DiaryDto> Archive(long id)
        {
            var userId = GetUserId();
            var diary = _diaryService.Archive(id, userId);
            return Ok(diary);
        }

        private int GetUserId()
        {
            var rawId = User.Claims
                .FirstOrDefault(c => c.Type == "personId")
                ?.Value;

            if (string.IsNullOrWhiteSpace(rawId))
                throw new UnauthorizedAccessException("User id not found in token.");

            return int.Parse(rawId);
        }
    }
}
