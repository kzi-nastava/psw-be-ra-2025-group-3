using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public
{
    public interface IBlogService
    {
        BlogDto CreateBlog(BlogDto blog);
        BlogDto UpdateBlog(BlogDto blog);
        BlogDto ChangeStatus(long blogId, int userId, int newStatus);
        List<BlogDto> GetUserBlogs(int userId);
        List<BlogDto> GetAllBlogs();


        CommentDto AddComment(long blogId, int userId, string text);
        CommentDto EditComment(long blogId, long commentId, int userId, string text);
        void DeleteComment(long blogId, long commentId, int userId);


        BlogVoteStateDto Vote(long blogId, int userId, bool isUpvote);
        BlogVoteStateDto GetUserVoteState(long blogId, int userId);

    }
}