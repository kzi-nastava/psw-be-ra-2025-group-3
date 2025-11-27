using Explorer.Stakeholders.Infrastructure.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageStorageService _imageStorageService;

        public ImagesController(IImageStorageService imageStorageService)
        {
            _imageStorageService = imageStorageService;
        }

        // POST /api/images/upload-single/club
        // POST /api/images/upload-single/blog
        // POST /api/images/upload-single/profile
        [HttpPost("upload-single/{category}")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<ActionResult<string>> UploadSingle(
            [FromRoute] string category,
            [FromForm] List<IFormFile> files,
            CancellationToken cancellationToken)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No file uploaded.");
            if (files.Count > 1)
                return BadRequest("Only one file allowed for this endpoint.");

            if (!TryParseCategory(category, out var imageCategory))
                return BadRequest("Invalid image category.");

            try
            {
                var file = files[0];
                var url = await _imageStorageService.SaveImageAsync(file, imageCategory, cancellationToken);
                return Ok(url);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/images/upload-multiple/club
        // POST /api/images/upload-multiple/blog
        // POST /api/images/upload-multiple/profile
        [HttpPost("upload-multiple/{category}")]
        [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
        public async Task<ActionResult<IReadOnlyList<string>>> UploadMultiple(
            [FromRoute] string category,
            [FromForm] List<IFormFile> files,
            CancellationToken cancellationToken)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            if (!TryParseCategory(category, out var imageCategory))
                return BadRequest("Invalid image category.");

            try
            {
                var urls = await _imageStorageService.SaveImagesAsync(files, imageCategory, cancellationToken);
                return Ok(urls);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static bool TryParseCategory(string category, out ImageCategory imageCategory)
        {
            return Enum.TryParse<ImageCategory>(category, true, out imageCategory);
        }
    }
}