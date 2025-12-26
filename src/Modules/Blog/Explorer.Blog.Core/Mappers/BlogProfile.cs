using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain.Blogs;
using BlogEntity = Explorer.Blog.Core.Domain.Blogs.Blog;
using BlogImageEntity = Explorer.Blog.Core.Domain.Blogs.BlogImage;

using System;
using System.Net;
using System.Text.RegularExpressions;

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
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
                // ✅ NEW: estimated read time
                .ForMember(dest => dest.EstimatedReadMinutes,
                    opt => opt.MapFrom(src => CalculateReadMinutes(src.Description)));

            CreateMap<BlogImageDto, BlogImageEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());  // Id se generiše u bazi

            CreateMap<BlogImageEntity, BlogImageDto>();

            // za komentare
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
        }

        private static int CalculateReadMinutes(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 1;

            // 1) Strip HTML tags + decode entities
            var s = Regex.Replace(text, "<[^>]+>", " ");
            s = WebUtility.HtmlDecode(s);

            // 2) Remove common markdown noise
            s = Regex.Replace(s, @"```[\s\S]*?```", " ");          // code blocks
            s = Regex.Replace(s, @"`[^`]*`", " ");                 // inline code
            s = Regex.Replace(s, @"!\[.*?\]\(.*?\)", " ");         // markdown images
            s = Regex.Replace(s, @"\[(.*?)\]\((.*?)\)", "$1");     // links -> text
            s = Regex.Replace(s, @"[#>*_~]", " ");                 // markdown chars
            s = Regex.Replace(s, @"\s+", " ").Trim();              // normalize whitespace

            if (string.IsNullOrWhiteSpace(s)) return 1;

            var wordCount = s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            // Standard: 200 wpm
            return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
        }
    }
}
