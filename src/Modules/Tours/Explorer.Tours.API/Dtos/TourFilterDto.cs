using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourFilterDto
{
    public string? Name { get; set; }
    public List<string>? Tags { get; set; }
    public int? MinDifficulty { get; set; } // 0 = Easy, 1 = Medium, 2 = Hard
    public int? MaxDifficulty { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public double? MinRating { get; set; } // 0-5
}
