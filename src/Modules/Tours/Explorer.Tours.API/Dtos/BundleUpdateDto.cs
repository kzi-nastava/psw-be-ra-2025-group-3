using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class BundleUpdateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<long> TourIds { get; set; }
    }
}