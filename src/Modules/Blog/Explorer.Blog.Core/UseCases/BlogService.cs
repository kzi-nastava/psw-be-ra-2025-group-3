using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using Explorer.Blog.Core.Domain.Blogs;

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

        public BlogDto ChangeStatus(long blogId, int userId, int newStatus)
        {
            var blog = _repository.GetById(blogId);

            if (blog.AuthorId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this blog.");

            var updated = _repository.UpdateStatus(blogId, newStatus);
            return _mapper.Map<BlogDto>(updated);
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
            
            if (blog.Status == 2 || blog.Status == 3)
                throw new InvalidOperationException("Voting is not allowed on archived or read-only blogs.");
            if (blog.Status != 1 && blog.Status != 4 && blog.Status != 5)
                throw new InvalidOperationException("Voting is allowed only on published/active/famous blogs.");

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

        public List<BlogDto> GetAllBlogs()
        {
            var blogs = _repository.GetAll()
                .Where(b => b.Status == 1 || b.Status == 2 || b.Status == 3 || b.Status == 4 || b.Status == 5)
                .ToList();

            return _mapper.Map<List<BlogDto>>(blogs);
        }
    }
}