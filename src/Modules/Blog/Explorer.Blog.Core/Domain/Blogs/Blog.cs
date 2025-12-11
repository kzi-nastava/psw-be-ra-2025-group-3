using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Core.Domain.Blogs
{
    // Status:
    // 0 = Draft, 1 = Published, 2 = Archived, 3 = ReadOnly, 4 = Active, 5 = Famous
    public class Blog : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public int AuthorId { get; private set; }
        public int Status { get; private set; }
        public List<BlogImage> Images { get; private set; }

        public List<BlogRating> Ratings { get; private set; }

        // Placeholder za broj komentara
        public int CommentsCount { get; private set; }

        public Blog()
        {
            Images = new List<BlogImage>();
            Status = 0;
            Ratings = new List<BlogRating>();
            CommentsCount = 0;
        }

        public Blog(string title, string description, int authorId, List<BlogImage> images = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;
            CreationDate = DateTime.UtcNow;
            AuthorId = authorId;
            Status = 0;
            Images = images ?? new List<BlogImage>();
            Ratings = new List<BlogRating>();
            CommentsCount = 0;
        }

        public void Update(string title, string description, List<BlogImage> newImages = null)
        {
            if (Status == 3)
                throw new InvalidOperationException("Cannot modify a read-only blog.");

            if (Status == 2)
                throw new InvalidOperationException("Cannot modify an archived blog.");

            if (Status == 1 || Status == 3 || Status == 4)
            {
                if (title != Title)
                    throw new InvalidOperationException("Cannot change title of a published blog.");

                if (newImages != null && newImages.Any())
                    throw new InvalidOperationException("Cannot change images of a published blog.");

                Description = description;
                LastModifiedDate = DateTime.UtcNow;
                return;
            }

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;
            LastModifiedDate = DateTime.UtcNow;

            if (newImages != null)
            {
                Images.Clear();
                Images.AddRange(newImages);
            }
        }

        public void ChangeStatus(int newStatus)
        {
            if (newStatus < 0 || newStatus > 5)
                throw new ArgumentException("Status mora biti 0, 1, 2, 3, 4 ili 5");
            Status = newStatus;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddImage(BlogImage image)
        {
            if (Status == 3)
                throw new InvalidOperationException("Cannot add images to a read-only blog.");

            if (Status == 2)
                throw new InvalidOperationException("Cannot add images to an archived blog.");

            if (Status == 1 || Status == 4 || Status == 5)
                throw new InvalidOperationException("Cannot add images to a published blog.");

            if (image == null)
                throw new ArgumentException("Image cannot be null");

            Images.Add(image);
        }

        public void RemoveImage(long imageId)
        {
            if (Status == 3)
                throw new InvalidOperationException("Cannot remove images from a read-only blog.");

            if (Status == 2)
                throw new InvalidOperationException("Cannot remove images from an archived blog.");

            if (Status == 1 || Status == 4 || Status == 5)
                throw new InvalidOperationException("Cannot remove images from a published blog.");

            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
                Images.Remove(image);
        }

        public void Rate(int userId, VoteType voteType, DateTime now)
        {
            if (Status != 1 && Status != 4 && Status != 5)
                throw new InvalidOperationException("Only published blogs can be rated.");
            var existingVote = Ratings.FirstOrDefault(r => r.UserId == userId);

            if (Status == 3)
                throw new InvalidOperationException("Voting is not allowed on read-only blogs.");

            if (existingVote == null)
            {
                var rating = new BlogRating(this.Id, userId, voteType, now);
                Ratings.Add(rating);
            }

            else if (existingVote.VoteType == voteType)
            {
                Ratings.Remove(existingVote);
            }
            else
            {
                existingVote.ChangeVote(voteType, now);
            }



            RecalculateStatus();
        }

        public void SetCommentsCount(int newCount)
        {
            if (newCount < 0) newCount = 0;

            // Read-only blogovi ne dozvoljavaju komentarisanje (promenu broja komentara)
            if (Status == 3)
                throw new InvalidOperationException("Comments are not allowed on read-only blogs.");

            CommentsCount = newCount;
            RecalculateStatus();
        }

        public int GetScore()
        {
            return Ratings.Sum(r => (int)r.VoteType);
        }

        private void RecalculateStatus()
        {
            var score = GetScore();
            // Pravila:
            // - ReadOnly: score < -10
            // - Active: score > 100 AND comments > 10
            // - Famous: score > 500 AND comments > 30
            // Napomena: ReadOnly ima prioritet (zatvara blog).
            if (score < -10)
            {                
                Status = 3;
            }
            else if (score > 500 && CommentsCount > 30)
            {
                Status = 5;
            }
            else if (score > 100 && CommentsCount > 10)
            {
                Status = 4;
            }
            else
            {
                // Ako uslovi nisu ispunjeni onda ne radi nista
            }

            LastModifiedDate = DateTime.UtcNow;
        }
    }
}