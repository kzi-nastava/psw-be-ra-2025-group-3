using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class MonumentDto
    {
        public long Id { get; set; }               
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }        
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
