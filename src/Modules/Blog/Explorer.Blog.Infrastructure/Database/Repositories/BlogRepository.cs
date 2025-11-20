using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogContext _context;

        public BlogRepository(BlogContext context)
        {
            _context = context;
        }

        public BlogEntity Add(BlogEntity blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return blog;
        }

        public BlogEntity Modify(BlogEntity blog)
        {
            _context.Entry(blog).State = EntityState.Modified;
            _context.SaveChanges();
            return blog;
        }

        public BlogEntity GetById(long id)
        {
            var blog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == id);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {id} not found.");

            return blog;
        }

        public List<BlogEntity> GetByAuthor(int authorId)
        {
            return _context.Blogs
                .Include(b => b.Images)
                .Where(b => b.AuthorId == authorId)
                .ToList();
        }
    }
}