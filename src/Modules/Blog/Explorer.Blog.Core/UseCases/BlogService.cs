using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.Blogs;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
            if (blog.Status == (int)BlogStatus.Archived)
                throw new InvalidOperationException("Voting is not allowed on archived blogs.");
            if (blog.Status == (int)BlogStatus.ReadOnly)
                throw new InvalidOperationException("Voting is not allowed on read-only blogs.");
            if (blog.Status != (int)BlogStatus.Active && blog.Status != (int)BlogStatus.Famous && blog.Status != (int)BlogStatus.Published)
                throw new InvalidOperationException("Voting is allowed only on published/active/famous blogs.");

            var voteType = isUpvote ? VoteType.Upvote : VoteType.Downvote;

            blog.Rate(userId, voteType, DateTime.Now);

            _repository.SaveChanges();

            return BuildRatingStateDto(blog, userId, blog.Status);
        }

        private static BlogVoteStateDto BuildRatingStateDto(BlogEntity blog, int userId, int status)
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
                DownvoteCount = downCount,
                BlogStatus = status
            };
        }

        public BlogVoteStateDto GetUserVoteState(long blogId, int userId)
        {
            var blog = _repository.GetById(blogId);
            if (blog == null)
                throw new ArgumentException($"Blog with id {blogId} not found.");

            return BuildRatingStateDto(blog, userId, blog.Status);
        }

        public List<BlogDto> GetAllBlogs()
        {
            var blogs = _repository.GetAll()
                .Where(b => b.Status != (int)BlogStatus.Draft)
                .ToList();

            return _mapper.Map<List<BlogDto>>(blogs);
        }

        public CommentDto AddComment(long blogId, int userId, string text)
        {
            var blog = _repository.GetById(blogId);

            blog.AddComment(userId, text);

            // ✅ umesto Modify (koji dira Images i pravi haos), samo snimi promenu
            _repository.SaveChanges();

            var created = blog.Comments.Last();
            return _mapper.Map<CommentDto>(created);
        }


        public CommentDto EditComment(long blogId, long commentId, int userId, string text)
        {
            var blog = _repository.GetById(blogId);
            blog.EditComment(commentId, userId, text);
             _repository.SaveChanges();

            var updated = blog.Comments.First(c => c.Id == commentId);
            return _mapper.Map<CommentDto>(updated);
        }

        public void DeleteComment(long blogId, long commentId, int userId)
        {
            var blog = _repository.GetById(blogId);
            blog.DeleteComment(commentId, userId);
            _repository.SaveChanges();
        }

        public List<CommentDto> GetComments(long blogId)
        {
            var blog = _repository.GetById(blogId);

            return blog.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c => _mapper.Map<CommentDto>(c))
                .ToList();
        }


    }
}