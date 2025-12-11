using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public List<string>? Tags { get; set; }
    public List<TourDurationDto> TourDurations { get; set; }
}
