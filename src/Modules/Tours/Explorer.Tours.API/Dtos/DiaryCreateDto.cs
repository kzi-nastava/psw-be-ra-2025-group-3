using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class DiaryCreateDto
    {
        public string Title { get; set; }
        public string Country { get; set; }
        public string? City { get; set; }
    }
}
