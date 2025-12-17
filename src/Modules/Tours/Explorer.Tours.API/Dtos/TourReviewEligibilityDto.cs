using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourReviewEligibilityDto
{
    public bool CanReview { get; set; }
    public string? ReasonIfNot { get; set; }
    public double CurrentProgress { get; set; }
    public int DaysSinceLastActivity { get; set; }
}