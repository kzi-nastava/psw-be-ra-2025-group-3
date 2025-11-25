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
        List<BlogDto> GetUserBlogs(int userId);
    }
}