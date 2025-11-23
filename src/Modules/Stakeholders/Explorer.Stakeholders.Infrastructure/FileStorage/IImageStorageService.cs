using Microsoft.AspNetCore.Http;

namespace Explorer.Stakeholders.Infrastructure.FileStorage
{
    public enum ImageCategory
    {
        Club,
        Blog,
        Profile
    }

    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(
            IFormFile file,
            ImageCategory category,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> SaveImagesAsync(
            IEnumerable<IFormFile> files,
            ImageCategory category,
            CancellationToken cancellationToken = default);
    }
}