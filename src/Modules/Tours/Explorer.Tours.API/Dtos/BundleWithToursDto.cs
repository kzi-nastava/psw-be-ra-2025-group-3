using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Tours.API.Dtos
{
    public class BundleWithToursDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public long AuthorId { get; set; }
        public List<TourDto> Tours { get; set; }
        public decimal TotalToursPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}