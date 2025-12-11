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

        private Comment() { } 

        public Comment(long blogId, int authorId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Comment cannot be empty.");

            BlogId = blogId;
            AuthorId = authorId;
            Text = text;
            CreatedAt = DateTime.UtcNow;
        }

        public void Edit(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("Comment cannot be empty.");

            Text = newText;
            EditedAt = DateTime.UtcNow;
        }

        public bool CanModify()
        {
            return DateTime.UtcNow - CreatedAt <= TimeSpan.FromMinutes(15);
        }
    }
}
