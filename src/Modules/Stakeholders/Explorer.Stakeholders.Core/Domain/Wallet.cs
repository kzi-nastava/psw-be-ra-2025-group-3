using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Wallet : Entity
    {
        public long PersonId { get; private set; }
        public int BalanceAc { get; private set; }

        private Wallet() { }

        public Wallet(long personId)
        {
            if (personId == 0) throw new ArgumentException("Invalid personId.");
            PersonId = personId;
            BalanceAc = 0;
        }

        public void AddAc(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be > 0.");
            BalanceAc += amount;
        }
    }
}
