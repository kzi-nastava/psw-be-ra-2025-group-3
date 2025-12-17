using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class PersonCreateDto
    {
        public long UserId { get; set; }  // ID iz User servisa
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

       
    }
}
