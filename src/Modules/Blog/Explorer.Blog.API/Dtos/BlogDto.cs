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
        public BlogStatus Status { get; set; } 
        public List<BlogImageDto> Images { get; set; } = new List<BlogImageDto>();
    }
}