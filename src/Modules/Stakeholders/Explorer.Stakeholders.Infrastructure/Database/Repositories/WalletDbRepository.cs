using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class WalletDbRepository : IWalletRepository
    {
        private readonly StakeholdersContext _context;
        public WalletDbRepository(StakeholdersContext context) => _context = context;

        public Wallet? GetByPersonId(long personId)
            => _context.Wallets.SingleOrDefault(w => w.PersonId == personId);

        public Wallet Create(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet;
        }

        public Wallet Update(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
            return wallet;
        }
    }
}
