using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourReviewUpdateDto
{
    public long ReviewId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
