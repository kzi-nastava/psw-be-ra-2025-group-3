using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IWalletService
    {
        WalletDto GetMyWallet(long personId);
        WalletDto TopUp(long adminPersonId, long touristPersonId, int amountAc);
    }
}