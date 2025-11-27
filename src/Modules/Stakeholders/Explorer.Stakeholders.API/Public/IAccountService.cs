using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IAccountService
    {
        AccountDto Create(AccountCreateDto dto);
        List<AccountDto> GetAll();
        AccountDto Get(int id);
        void Block(int id);
        void Unblock(int id);
    }
}
