using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class BlogImage : Entity
    {
        public string ImageUrl { get; init; }
        public long BlogId { get; init; }

        public BlogImage() { }

        public BlogImage(string imageUrl, long blogId)
        {
            ImageUrl = imageUrl;
            BlogId = blogId;
        }
    }
}