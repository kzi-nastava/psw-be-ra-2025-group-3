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


        public BlogVoteStateDto Vote(long blogId, int userId, bool isUpvote)
        {
            var blog = _repository.GetById(blogId);
            if (blog == null)
                throw new ArgumentException($"Blog with id {blogId} not found.");

            var voteType = isUpvote ? VoteType.Upvote : VoteType.Downvote;

            blog.Rate(userId, voteType, DateTime.Now);

            _repository.Modify(blog);

            return BuildRatingStateDto(blog, userId);
        }

        private static BlogVoteStateDto BuildRatingStateDto(BlogEntity blog, int userId)
        {
            var userVote = blog.Ratings.FirstOrDefault(r => r.UserId == userId);

            var upCount = blog.Ratings.Count(r => r.VoteType == VoteType.Upvote);
            var downCount = blog.Ratings.Count(r => r.VoteType == VoteType.Downvote);

            return new BlogVoteStateDto
            {
                BlogId = blog.Id,
                IsUpvote = userVote == null
                    ? (bool?)null
                    : userVote.VoteType == VoteType.Upvote,
                Score = blog.GetScore(),
                UpvoteCount = upCount,
                DownvoteCount = downCount
            };
        }

        public BlogVoteStateDto GetUserVoteState(long blogId, int userId)
        {
            var blog = _repository.GetById(blogId);
            if (blog == null)
                throw new ArgumentException($"Blog with id {blogId} not found.");

            return BuildRatingStateDto(blog, userId);
        }
    }
}