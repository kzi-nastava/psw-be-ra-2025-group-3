using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class Blog : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int AuthorId { get; private set; }
        public List<BlogImage> Images { get; private set; }

        public Blog()
        {
            Images = new List<BlogImage>();
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
        }

        public void Update(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            Description = description;
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
    }
}
