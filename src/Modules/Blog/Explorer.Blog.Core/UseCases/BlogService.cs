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
            var entity = _mapper.Map<BlogEntity>(blogDto);
            var created = _repository.Add(entity);
            return _mapper.Map<BlogDto>(created);
        }

        public BlogDto UpdateBlog(BlogDto blogDto)
        {
            var entity = _mapper.Map<BlogEntity>(blogDto);
            var updated = _repository.Modify(entity);
            return _mapper.Map<BlogDto>(updated);
        }

        public List<BlogDto> GetUserBlogs(int userId)
        {
            var blogs = _repository.GetByAuthor(userId);
            return _mapper.Map<List<BlogDto>>(blogs);
        }

        public List<BlogDto> GetAllBlogs()
        {
            var blogs = _repository.GetAll();
            return _mapper.Map<List<BlogDto>>(blogs);
        }
    }
}
