using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
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
            var blog = _mapper.Map<BlogEntity>(blogDto);
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