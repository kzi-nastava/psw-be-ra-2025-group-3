using Explorer.Blog.API.Dtos;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogRepository
    {
        BlogEntity Add(BlogEntity blog);
        BlogEntity Modify(BlogEntity blog);
        BlogEntity UpdateStatus(long blogId, int newStatus);
        BlogEntity GetById(long id);
        List<BlogEntity> GetByAuthor(int authorId);
        List<BlogEntity> GetAll();
    }
}