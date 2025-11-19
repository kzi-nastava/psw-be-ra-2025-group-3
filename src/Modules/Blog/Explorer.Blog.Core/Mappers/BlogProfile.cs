using AutoMapper;
using Explorer.Blog.API.Dtos;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

namespace Explorer.Blog.Core.Mappers
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<BlogDto, BlogEntity>().ReverseMap();
            CreateMap<BlogImageDto, BlogImageEntity>().ReverseMap();
        }
    }
}