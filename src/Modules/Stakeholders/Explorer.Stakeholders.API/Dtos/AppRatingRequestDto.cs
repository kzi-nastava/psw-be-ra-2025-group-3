using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class AppRatingRequestDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}