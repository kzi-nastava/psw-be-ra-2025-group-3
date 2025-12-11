using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Blog.API.Dtos; 
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class Blog : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public int AuthorId { get; private set; }
        public int Status { get; private set; }
        public List<BlogImage> Images { get; private set; }
        public List<Comment> Comments { get; private set; } = new();

        public Blog()
        {
            Images = new List<BlogImage>();
            Status = 0;
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
        }

        public void Update(string title, string description, List<BlogImage> newImages = null)
        {
            if (Status == 2)
                throw new InvalidOperationException("Cannot modify an archived blog.");

            if (Status == 1)
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
            if (newStatus < 0 || newStatus > 2)
                throw new ArgumentException("Status mora biti 0, 1 ili 2");
            Status = newStatus;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddImage(BlogImage image)
        {
            if (Status == 2)
                throw new InvalidOperationException("Cannot add images to an archived blog.");

            if (Status == 1)
                throw new InvalidOperationException("Cannot add images to a published blog.");

            if (image == null)
                throw new ArgumentException("Image cannot be null");

            Images.Add(image);
        }

        public void RemoveImage(long imageId)
        {
            if (Status == 2)
                throw new InvalidOperationException("Cannot remove images from an archived blog.");

            if (Status == 1)
                throw new InvalidOperationException("Cannot remove images from a published blog.");

            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
                Images.Remove(image);
        }


        //metode za komentare 

        public void AddComment(int authorId, string text)
        {
            if (Status != 1)   // 1 = published
                throw new InvalidOperationException("Cannot comment on an unpublished blog.");

            var comment = new Comment(this.Id, authorId, text);
            Comments.Add(comment);
        }

        public void EditComment(long commentId, int authorId, string newText)
        {
            var comment = Comments.FirstOrDefault(c => c.Id == commentId)
                ?? throw new InvalidOperationException("Comment not found.");

            if (comment.AuthorId != authorId)
                throw new InvalidOperationException("Not your comment.");

            if (!comment.CanModify())
                throw new InvalidOperationException("Editing period expired.");

            comment.Edit(newText);
        }

        public void DeleteComment(long commentId, int authorId)
        {
            var comment = Comments.FirstOrDefault(c => c.Id == commentId)
                ?? throw new InvalidOperationException("Comment not found.");

            if (comment.AuthorId != authorId)
                throw new InvalidOperationException("Not your comment.");

            if (!comment.CanModify())
                throw new InvalidOperationException("Deleting period expired.");

            Comments.Remove(comment);
        }

    }
}