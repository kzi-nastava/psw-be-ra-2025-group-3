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
                // ✅ NOVO: procenjeno vreme čitanja (min read)
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

            // Skini HTML tagove ako ih ima
            var noHtml = Regex.Replace(text, "<.*?>", " ");
            noHtml = WebUtility.HtmlDecode(noHtml);

            // Brojanje reči (slova/brojevi/apostrof)
            var wordCount = Regex.Matches(noHtml, @"\b[\p{L}\p{N}']+\b").Count;

            // Standard: ~200 reči u minuti
            var minutes = (int)Math.Ceiling(wordCount / 200.0);

            return Math.Max(1, minutes);
        }
    }
}
