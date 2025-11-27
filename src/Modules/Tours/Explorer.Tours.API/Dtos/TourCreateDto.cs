using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; } // 0=Easy, 1=Medium, 2=Hard
    public List<string>? Tags { get; set; }
}
