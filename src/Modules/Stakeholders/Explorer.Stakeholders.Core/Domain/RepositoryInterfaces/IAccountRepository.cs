using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAccountRepository
    {
        bool Exists(string username);
        Account Create(Account account);
        Account? Get(long id);
        Account? GetByUsername(string username);
        List<Account> GetAll();
        void Update(Account account);
    }
}
