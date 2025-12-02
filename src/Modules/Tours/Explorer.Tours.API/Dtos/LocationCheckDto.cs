using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class LocationCheckDto
{

    public long TourId { get; set; }  //dodato 
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
}
