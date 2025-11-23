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

        /// <summary>
        /// NAJSIGURNIJA VERZIJA - Pravilno rukuje slikama i EF Core tracking-om
        /// </summary>
        public BlogEntity Modify(BlogEntity blog)
        {
            // 1. Dohvati postojeći blog sa slikama
            var existingBlog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == blog.Id);

            if (existingBlog == null)
                throw new KeyNotFoundException($"Blog sa ID {blog.Id} nije pronađen.");

            // 2. Ažuriraj Title i Description direktno (BEZ slika u ovom koraku)
            existingBlog.Update(blog.Title, blog.Description);

            // 3. Ručno obriši stare slike iz baze
            var existingImages = _context.Set<BlogImageEntity>()
                .Where(img => img.BlogId == blog.Id)
                .ToList();

            _context.RemoveRange(existingImages);

            // 4. Dodaj nove slike
            if (blog.Images != null && blog.Images.Any())
            {
                foreach (var image in blog.Images)
                {
                    _context.Set<BlogImageEntity>().Add(new BlogImageEntity(image.ImageUrl, blog.Id));
                }
            }

            // 5. Sačuvaj izmene
            _context.SaveChanges();

            // 6. Ponovo dohvati blog sa novim slikama za response
            var updatedBlog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == blog.Id);

            return updatedBlog;
        }

        public BlogEntity GetById(long id)
        {
            var blog = _context.Blogs
                .Include(b => b.Images)
                .FirstOrDefault(b => b.Id == id);

            if (blog == null)
                throw new KeyNotFoundException($"Blog sa ID {id} nije pronađen.");

            return blog;
        }

        public List<BlogEntity> GetByAuthor(int authorId)
        {
            return _context.Blogs
                .Include(b => b.Images)
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreationDate)
                .ToList();
        }
    }
}