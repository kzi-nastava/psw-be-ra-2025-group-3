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
        public List<Comment> Comments { get; private set; }
        public List<BlogRating> Ratings { get; private set; }

        public Blog()
        {
            Images = new List<BlogImage>();
            Status = (int)BlogStatus.Draft;
            Comments = new List<Comment>();
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
            Comments = new List<Comment>();     // ✅ bitno
            Ratings = new List<BlogRating>();
            Status = (int)BlogStatus.Draft;
        }

        public void Update(string title, string description, List<BlogImage> newImages = null)
        {
            if (Status == (int)BlogStatus.Archived || Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Cannot modify an archived/read-only blog.");

            // Published (i Active/Famous) – ne dozvoli promenu title i slika (kao što već radiš)
            if (Status == (int)BlogStatus.Published || Status == (int)BlogStatus.Active || Status == (int)BlogStatus.Famous)
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
            // Ovo je ručna promena Draft/Published/Archived (postojeća logika)
            if (newStatus < 0 || newStatus > 2)
                throw new ArgumentException("Status mora biti 0, 1 ili 2");

            // Ne dozvoli ručno menjanje ako je ReadOnly
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Cannot change status of a read-only blog.");

            Status = newStatus;
            LastModifiedDate = DateTime.UtcNow;

            // ✅ posle ručne promene, samo osveži “automatske” statuse ako je published
            if (Status == (int)BlogStatus.Published)
            {
                RecalculateStatus();
            }
        }

        public void AddImage(BlogImage image)
        {
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Cannot add images to a read-only blog.");
            if (Status == (int)BlogStatus.Archived)
                throw new InvalidOperationException("Cannot add images to an archived blog.");

            if (Status == (int)BlogStatus.Published || Status == (int)BlogStatus.Active || Status == (int)BlogStatus.Famous)
                throw new InvalidOperationException("Cannot add images to a published blog.");

            if (image == null)
                throw new ArgumentException("Image cannot be null");

            Images.Add(image);
        }

        public void RemoveImage(long imageId)
        {
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Cannot remove images from a read-only blog.");
            if (Status == (int)BlogStatus.Archived)
                throw new InvalidOperationException("Cannot remove images from an archived blog.");

            if (Status == (int)BlogStatus.Published || Status == (int)BlogStatus.Active || Status == (int)BlogStatus.Famous)
                throw new InvalidOperationException("Cannot remove images from a published blog.");

            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
                Images.Remove(image);
        }

        // ================== COMMENTS ==================
        public void AddComment(int userId, string text)
        {
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Blog is read-only.");

            Comments ??= new List<Comment>();

            // nema LINQ nad Comments pre Add
            Comments.Add(new Comment(Id, userId, text));

            RecalculateStatus();
        }

        public void EditComment(long commentId, int userId, string newText)
        {
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Blog is read-only.");

            var comment = Comments.FirstOrDefault(c => c.Id == commentId)
                ?? throw new InvalidOperationException("Comment not found.");

            comment.Edit(userId, newText);
        }

        public void DeleteComment(long commentId, int userId)
        {
            if (Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Blog is read-only.");

            if (Comments == null)
                throw new InvalidOperationException("Comments not loaded.");

            var commentsSnapshot = Comments.ToList();
            var comment = commentsSnapshot.FirstOrDefault(c => c.Id == commentId)
                ?? throw new InvalidOperationException("Comment not found.");

            comment.EnsureCanDelete(userId);
            Comments.Remove(comment);

            RecalculateStatus();
        }

        // ================== RATING ==================

        public void Rate(int userId, VoteType voteType, DateTime now)
        {
            if (Status != (int)BlogStatus.Published && Status != (int)BlogStatus.Active && Status != (int)BlogStatus.Famous)
                throw new InvalidOperationException("Only published blogs can be rated.");
            var existingVote = Ratings.FirstOrDefault(r => r.UserId == userId);

            if (Status == (int)BlogStatus.ReadOnly)
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
                Status = (int)BlogStatus.ReadOnly;
            }
            else if (score > 500 && this.Comments.Count > 30)
            {
                Status = (int)BlogStatus.Famous;
            }
            else if (score > 100 && this.Comments.Count > 10)
            {
                Status = (int)BlogStatus.Active;
            }
            else
            {
                Status = (int)BlogStatus.Published;
            }

            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
