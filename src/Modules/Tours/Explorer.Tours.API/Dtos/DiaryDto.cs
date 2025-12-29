using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class DiaryDto
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; } // 0 Draft, 1 Archived

        public string Country { get; set; }
        public string? City { get; set; }

        public int TouristId { get; set; } // owner (možeš sakriti na frontu ako želiš)
    }
}
