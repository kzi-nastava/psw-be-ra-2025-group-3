using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.Blogs;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly IMapper _mapper;

        public BlogService(IBlogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public BlogDto CreateBlog(BlogDto blogDto)
        {
            var blog = _mapper.Map<BlogEntity>(blogDto);
            var createdBlog = _repository.Add(blog);
            return _mapper.Map<BlogDto>(createdBlog);
        }

        public BlogDto UpdateBlog(BlogDto blogDto)
        {
            // ✅ REŠENJE: Mapiraj ceo DTO u entity (uključujući slike)
            // Ali NE pozivaj Update() metodu koja će dodavati slike u existingBlog
            var blog = _mapper.Map<BlogEntity>(blogDto);

            // Repository.Modify će sveHandlovati:
            // 1. Dohvatiti existingBlog
            // 2. Update-ovati Title i Description
            // 3. Obrisati stare slike
            // 4. Dodati nove slike iz blog.Images
            var updatedBlog = _repository.Modify(blog);

            return _mapper.Map<BlogDto>(updatedBlog);
        }

        public List<BlogDto> GetUserBlogs(int userId)
        {
            var blogs = _repository.GetByAuthor(userId);
            return _mapper.Map<List<BlogDto>>(blogs);
        }
    }
}