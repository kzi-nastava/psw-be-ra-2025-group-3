using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IWalletRepository
    {
        Wallet Create(Wallet wallet);
        Wallet? GetByPersonId(long personId);
        Wallet Update(Wallet wallet);
    }
}