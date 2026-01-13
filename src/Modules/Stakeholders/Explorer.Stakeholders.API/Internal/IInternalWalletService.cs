using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IInternalWalletService
    {
        WalletDto GetWallet(long personId);
        WalletDto DeductAc(long personId, decimal amountAc);
    }
}