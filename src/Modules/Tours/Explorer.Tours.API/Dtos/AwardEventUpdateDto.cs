using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Explorer.Tours.API.Dtos
{
    public class AwardEventUpdateDto
    {
        [Required]
        public long Id { get; set; } // Id je obavezan kod izmene
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Range(2020, 2100)]
        public int Year { get; set; }
        [Required]
        public DateTime VotingStartDate { get; set; }
        [Required]
        public DateTime VotingEndDate { get; set; }
    }
}
