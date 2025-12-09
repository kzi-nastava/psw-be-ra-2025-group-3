// Explorer.API/Controllers/Tourist/ReviewImageUploadController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/review-images")]
    [ApiController]
    public class ReviewImageUploadController : ControllerBase
    {
        private readonly string _uploadFolder;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ReviewImageUploadController(IWebHostEnvironment environment)
        {
            _uploadFolder = Path.Combine(environment.WebRootPath, "uploads", "review-images");

            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ReviewImageUploadResponse>> UploadImage(IFormFile file)  
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file uploaded" });
                }

                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { error = $"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024} MB" });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { error = "Invalid file type. Allowed types: " + string.Join(", ", _allowedExtensions) });
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/review-images/{fileName}";

                return Ok(new ReviewImageUploadResponse  
                {
                    ImageUrl = imageUrl,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error uploading image: " + ex.Message });
            }
        }

        [HttpDelete("{fileName}")]
        public ActionResult DeleteImage(string fileName)
        {
            try
            {
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest(new { error = "Invalid file name" });
                }

                var filePath = Path.Combine(_uploadFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { error = "File not found" });
                }

                System.IO.File.Delete(filePath);

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error deleting image: " + ex.Message });
            }
        }
    }

  
    public class ReviewImageUploadResponse
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}