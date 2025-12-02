using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Author_Tourist
{
    [Authorize(Policy = "touristOrAuthorPolicy")]
    [Route("api/images")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly string _uploadFolder;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ImageUploadController(IWebHostEnvironment environment)
        {
            // Kreiraj putanju do uploads/blog-images foldera
            _uploadFolder = Path.Combine(environment.WebRootPath, "uploads", "blog-images");

            // Kreiraj folder ako ne postoji
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ImageUploadResponse>> UploadImage(IFormFile file)
        {
            try
            {
                // Validacija
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

                // Generiši jedinstveno ime fajla
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_uploadFolder, fileName);

                // Sačuvaj fajl
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Vrati URL slike
                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/blog-images/{fileName}";

                return Ok(new ImageUploadResponse
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

    public class ImageUploadResponse
    {
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
    }
}