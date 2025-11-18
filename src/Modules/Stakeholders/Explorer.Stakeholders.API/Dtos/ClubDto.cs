using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long OwnerId { get; set; }
        public long FeaturedImageId { get; set; }
        public ClubImageDto FeaturedImage { get; set; }
        public List<ClubImageDto> GalleryImages { get; set; }
    }

    public class ClubImageDto
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class ClubCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FeaturedImageUrl { get; set; }
        public List<string>? GalleryImageUrls { get; set; }
    }

    public class ClubUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? PromoteGalleryImageId { get; set; }
        public string? NewFeaturedImageUrl { get; set; }
        public List<string>? NewGalleryImageUrls { get; set; }
        public List<long>? RemovedGalleryImageIds { get; set; }
    }
}
