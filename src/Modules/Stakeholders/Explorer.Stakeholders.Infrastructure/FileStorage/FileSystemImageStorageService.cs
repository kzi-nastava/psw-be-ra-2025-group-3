using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Explorer.Stakeholders.Infrastructure.FileStorage
{
    public class FileSystemImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _imagesRootFolder;
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public FileSystemImageStorageService(IWebHostEnvironment env)
        {
            _env = env;
            _imagesRootFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            Directory.CreateDirectory(_imagesRootFolder);
        }

        public async Task<string> SaveImageAsync(
            IFormFile file,
            ImageCategory category,
            CancellationToken cancellationToken = default)
        {
            ValidateImage(file);

            var categoryFolderName = GetCategoryFolderName(category); // "clubs", "blogs", "profiles"
            var categoryFolderPath = Path.Combine(_imagesRootFolder, categoryFolderName);
            Directory.CreateDirectory(categoryFolderPath);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(categoryFolderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"/images/{categoryFolderName}/{fileName}";
        }

        public async Task<IReadOnlyList<string>> SaveImagesAsync(
            IEnumerable<IFormFile> files,
            ImageCategory category,
            CancellationToken cancellationToken = default)
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file is { Length: > 0 })
                {
                    var url = await SaveImageAsync(file, category, cancellationToken);
                    urls.Add(url);
                }
            }

            return urls;
        }

        private static void ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            if (file.ContentType is null || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only image files are allowed.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File type '{extension}' is not allowed.");
        }

        private static string GetCategoryFolderName(ImageCategory category)
        {
            return category switch
            {
                ImageCategory.Club => "clubs",
                ImageCategory.Blog => "blogs",
                ImageCategory.Profile => "profiles",
                _ => "other"
            };
        }
    }
}