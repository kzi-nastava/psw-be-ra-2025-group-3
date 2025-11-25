using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogRepository
    {
        BlogEntity Add(BlogEntity blog);
        BlogEntity Modify(BlogEntity blog);
        BlogEntity GetById(long id);
        List<BlogEntity> GetByAuthor(int authorId);
    }
}