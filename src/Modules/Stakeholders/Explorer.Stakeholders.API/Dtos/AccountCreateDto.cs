using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class AccountCreateDto
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
