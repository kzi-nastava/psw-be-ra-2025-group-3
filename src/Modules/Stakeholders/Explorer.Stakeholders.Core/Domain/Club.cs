using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long OwnerId { get; private set; }
        public long? FeaturedImageId { get; private set; }
        public virtual ClubImage? FeaturedImage { get; private set; }
        public virtual List<ClubImage> Images { get; private set; }

        public Club()
        {
            Images = new List<ClubImage>();
        }

        public Club(string name, string description, long ownerId)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            Images = new List<ClubImage>();
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Club name is required.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Club description is required.");
        }

        public void Update(string name, string description)
        {
            Name = name;
            Description = description;
            Validate();
        }

        public void SetFeaturedImage(long? imageId)
        {
            FeaturedImageId = imageId;
        }

        public void AddGalleryImage(ClubImage image)
        {
            if (image == null)
                throw new ArgumentException("Image cannot be null.");

            Images.Add(image);
        }

        public void RemoveGalleryImage(long imageId)
        {
            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                throw new ArgumentException("Image not found in gallery.");

            Images.Remove(image);
        }

        public void PromoteGalleryImageToFeatured(long imageId)
        {
            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                throw new ArgumentException("Image not found in gallery.");
            
            FeaturedImageId = image.Id;
        }

        public void DemoteFeaturedToGallery()
        {
            if (FeaturedImage != null)
            {
                Images.Add(FeaturedImage);
                FeaturedImageId = 0;
            }
        }
    }
}
