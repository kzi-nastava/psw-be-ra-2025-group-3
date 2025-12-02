using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourExecutionCreateDto
{
    public long TourId { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
}
