using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

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
            var existingBlog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == blog.Id);

            if (existingBlog == null)
                throw new KeyNotFoundException($"Blog sa ID {blog.Id} nije pronađen.");

            existingBlog.Update(blog.Title, blog.Description);

            var existingImages = _context.Set<BlogImageEntity>()
                .Where(img => img.BlogId == blog.Id)
                .ToList();
            _context.RemoveRange(existingImages);

            if (blog.Images != null && blog.Images.Any())
            {
                var imagesToAdd = blog.Images.ToList();
                foreach (var image in imagesToAdd)
                {
                    _context.Set<BlogImageEntity>().Add(new BlogImageEntity(image.ImageUrl, blog.Id));
                }
            }

            _context.SaveChanges();

            var updatedBlog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == blog.Id);

            return updatedBlog;
        }

        /// <summary>
        /// ✅ NOVA METODA - Samo za promenu statusa
        /// </summary>
        public BlogEntity UpdateStatus(long blogId, int newStatus)
        {
            if (newStatus < 0 || newStatus > 2)
                throw new ArgumentException("Status mora biti 0, 1 ili 2");

            var blog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == blogId);

            if (blog == null)
                throw new KeyNotFoundException($"Blog sa ID {blogId} nije pronađen.");

            blog.ChangeStatus(newStatus);
            _context.SaveChanges();

            return blog;
        }

        public BlogEntity GetById(long id)
        {
            var blog = _context.Blogs
                .Include(b => b.Images)
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Id == id);

            if (blog == null)
                throw new KeyNotFoundException($"Blog sa ID {id} nije pronađen.");

            return blog;
        }

        public List<BlogEntity> GetByAuthor(int authorId)
        {
            return _context.Blogs
                .Include(b => b.Images)
                .Include(b => b.Comments)
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreationDate)
                .ToList();
        }

        public List<BlogEntity> GetAll()
        {
            return _context.Blogs
                .Include(b => b.Images)
                .Include(b => b.Comments)
                .OrderByDescending(b => b.CreationDate)
                .ToList();
        }
    }
}