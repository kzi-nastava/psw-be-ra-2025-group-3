using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubImage : Entity
    {
        public long ClubId { get; private set; }
        public string ImageUrl { get; private set; }
        public DateTime UploadedAt { get; private set; }

        public ClubImage() { }

        public ClubImage(long clubId, string imageUrl)
        {
            ClubId = clubId;
            ImageUrl = imageUrl;
            UploadedAt = DateTime.Now;

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL is required.");
        }
    }
}
