using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourReviewCreateDto
{
    public long TourId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
}
