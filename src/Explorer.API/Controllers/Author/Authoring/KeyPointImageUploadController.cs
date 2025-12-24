using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Author.Authoring
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/keypoints/images")]
    [ApiController]
    public class KeyPointImageUploadController : ControllerBase
    {
        private readonly string _uploadFolder;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public KeyPointImageUploadController(IWebHostEnvironment env)
        {
            _uploadFolder = Path.Combine(env.WebRootPath, "uploads", "keypoint-images");

            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<KeyPointImageUploadResponse>> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { error = "No file uploaded" });

                if (file.Length > MaxFileSize)
                    return BadRequest(new { error = "File too large" });

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return BadRequest(new { error = "Invalid file type" });

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                return Ok(new KeyPointImageUploadResponse
        {
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/uploads/keypoint-images/{fileName}",
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            var path = Path.Combine(_uploadFolder, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            System.IO.File.Delete(path);
            return Ok(new { message = "Deleted" });
        }
    }

    public class KeyPointImageUploadResponse
    {
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
    }
}
