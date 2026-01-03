using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class WalletTopUpDto
    {
        public long TouristUserId { get; set; }
        public int AmountAc { get; set; }
    }
}
