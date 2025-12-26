using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Explorer.Blog.API.Dtos
{
    public class BlogDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Naslov je obavezan")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Naslov mora biti između 1 i 200 karaktera")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis je obavezan")]
        [StringLength(10000, MinimumLength = 1, ErrorMessage = "Opis mora biti između 1 i 10000 karaktera")]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int AuthorId { get; set; }
        public int CommentsCount { get; set; }


        //[Range(0, 2, ErrorMessage = "Status mora biti 0 (Draft), 1 (Published) ili 2 (Archived)")]
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsFamous { get; set; }
        public List<BlogImageDto> Images { get; set; } = new List<BlogImageDto>();
        public int EstimatedReadMinutes { get; set; }

        public List<BlogVoteDto> Ratings { get; set; } = new List<BlogVoteDto>();
    }
}