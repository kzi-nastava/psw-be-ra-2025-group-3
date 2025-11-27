using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class AppRating : Entity
    {
        public long UserId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; set; }

        public AppRating(long userId, int rating, string? comment)
        {
            UserId = userId;
            Rating = rating;
            Comment = comment;
            CreatedAt = DateTime.Now;

            Validate();
        }

        public void Validate()
        {
            if (UserId == 0) throw new ArgumentException("Invalid UserId.");
            if (Rating < 1 || Rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");
            if (Comment != null && Comment.Length > 1000) throw new ArgumentException("Comment cannot be longer than 1000 characters.");
        }

    }
}