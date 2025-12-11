using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// ✅ AŽURIRANA METODA - Koristi UpdateStatus umesto Modify
        /// </summary>
        public BlogDto ChangeStatus(long blogId, int userId, int newStatus)
        {
            var blog = _repository.GetById(blogId);

            if (blog.AuthorId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this blog.");

            var updated = _repository.UpdateStatus(blogId, newStatus); // ✅ Koristi novu metodu
            return _mapper.Map<BlogDto>(updated);
        }

        public List<BlogDto> GetUserBlogs(int userId)
        {
            var blogs = _repository.GetByAuthor(userId);
            return _mapper.Map<List<BlogDto>>(blogs);
        }

        public List<BlogDto> GetAllBlogs()
        {
            var blogs = _repository.GetAll()
                .Where(b => b.Status == 1 || b.Status == 2)
                .ToList();

            return _mapper.Map<List<BlogDto>>(blogs);
        }

        public CommentDto AddComment(long blogId, int userId, string text)
        {
            var blog = _repository.GetById(blogId);
            blog.AddComment(userId, text);
            _repository.Modify(blog);

            var created = blog.Comments.Last();
            return _mapper.Map<CommentDto>(created);
        }

        public CommentDto EditComment(long blogId, long commentId, int userId, string text)
        {
            var blog = _repository.GetById(blogId);
            blog.EditComment(commentId, userId, text);
            _repository.Modify(blog);

            var updated = blog.Comments.First(c => c.Id == commentId);
            return _mapper.Map<CommentDto>(updated);
        }

        public void DeleteComment(long blogId, long commentId, int userId)
        {
            var blog = _repository.GetById(blogId);
            blog.DeleteComment(commentId, userId);
            _repository.Modify(blog);
        }

    }
}