using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Blog.API.Dtos; 
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
        public DateTime? LastModifiedDate { get; private set; }
        public int AuthorId { get; private set; }
        public BlogStatus Status { get; private set; } 
        public List<BlogImage> Images { get; private set; }

        public Blog()
        {
            Images = new List<BlogImage>();
            Status = BlogStatus.Draft;
        }

        public Blog(string title, string description, int authorId, List<BlogImage> images = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;
            CreationDate = DateTime.UtcNow;
            AuthorId = authorId;
            Status = BlogStatus.Draft;
            Images = images ?? new List<BlogImage>();
        }

        public void Update(string title, string description, List<BlogImage> newImages = null)
        {
            if (Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot modify an archived blog.");

            if (Status == BlogStatus.Published)
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

        public void ChangeStatus(BlogStatus newStatus)
        {
            Status = newStatus;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddImage(BlogImage image)
        {
            if (Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot add images to an archived blog.");

            if (Status == BlogStatus.Published)
                throw new InvalidOperationException("Cannot add images to a published blog.");

            if (image == null)
                throw new ArgumentException("Image cannot be null");

            Images.Add(image);
        }

        public void RemoveImage(long imageId)
        {
            if (Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot remove images from an archived blog.");

            if (Status == BlogStatus.Published)
                throw new InvalidOperationException("Cannot remove images from a published blog.");

            var image = Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
                Images.Remove(image);
        }
    }
}