using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class Comment : Entity
    {
        public long BlogId { get; private set; }
        public int AuthorId { get; private set; }
        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? EditedAt { get; private set; }

        private Comment() { } // EF

        public Comment(long blogId, int authorId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Comment text cannot be empty.");

            BlogId = blogId;
            AuthorId = authorId;
            Text = text;
            CreatedAt = DateTime.UtcNow;
        }

        public void Edit(int userId, string newText)
        {
            if (userId != AuthorId)
                throw new InvalidOperationException("You can edit only your own comment.");

            if (DateTime.UtcNow > CreatedAt.AddMinutes(15))
                throw new InvalidOperationException("Comment can be edited only within 15 minutes.");

            Text = newText;
            EditedAt = DateTime.UtcNow;
        }

        public void EnsureCanDelete(int userId)
        {
            if (userId != AuthorId)
                throw new InvalidOperationException("You can delete only your own comment.");

            if (DateTime.UtcNow > CreatedAt.AddMinutes(15))
                throw new InvalidOperationException("Comment can be deleted only within 15 minutes.");
        }
    }
}
