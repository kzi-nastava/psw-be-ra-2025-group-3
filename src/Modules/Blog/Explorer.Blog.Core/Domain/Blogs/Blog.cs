using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class Blog : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int AuthorId { get; private set; }
        public List<BlogImage> Images { get; private set; }

        public List<BlogRating> Ratings { get; private set; }

        public Blog()
        {
            Images = new List<BlogImage>();
            Ratings = new List<BlogRating>();
        }

        public Blog(string title, string description, int authorId, List<BlogImage> images = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;
            CreationDate = DateTime.UtcNow;
            AuthorId = authorId;
            Images = images ?? new List<BlogImage>();
            Ratings = new List<BlogRating>();
        }

        /// <summary>
        /// ✅ ISPRAVLJENA METODA - Sada prima i slike!
        /// </summary>
        public void Update(string title, string description, List<BlogImage> newImages = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;

            // ✅ Ako su prosleđene nove slike, zameni stare
            if (newImages != null)
            {
                Images.Clear();
                Images.AddRange(newImages);
            }
        }

        public void AddImage(BlogImage image)
        {
            if (image == null)
                throw new ArgumentException("Image cannot be null");

            Images.Add(image);
        }

        public void RemoveImage(long imageId)
        {
            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
                Images.Remove(image);
        }

        public void Rate(int userId, VoteType voteType, DateTime now)
        {
            // TODO: provera za status bloga (samo objavljeni)
            var existingVote = Ratings.FirstOrDefault(r => r.UserId == userId);

            if (existingVote == null)
            {
                var rating = new BlogRating(this.Id, userId, voteType, now);
                Ratings.Add(rating);
                return;
            }

            if (existingVote.VoteType == voteType)
            {
                Ratings.Remove(existingVote);
                return;
            }
            existingVote.ChangeVote(voteType, now);
        }

        public int GetScore()
        {
            return Ratings.Sum(r => (int)r.VoteType);
        }
    }
}