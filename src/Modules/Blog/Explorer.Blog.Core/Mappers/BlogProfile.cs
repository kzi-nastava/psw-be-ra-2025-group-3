using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain.Blogs;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

namespace Explorer.Blog.Core.Mappers
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<BlogDto, BlogEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))  // ✅ Uvek mapiraj Id
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));



            CreateMap<BlogEntity, BlogDto>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(d => d.IsActive,
                 opt => opt.MapFrom(s => s.GetScore() > 100 || s.Comments.Count > 10))
                .ForMember(d => d.IsFamous,
                 opt => opt.MapFrom(s => s.GetScore() > 500 && s.Comments.Count > 30))
               .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count));


            CreateMap<BlogImageDto, BlogImageEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());  // Id se generiše u bazi

            CreateMap<BlogImageEntity, BlogImageDto>();

            //za komentare
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
        }
    }
}