using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IPersonRepository _personRepo;
        private readonly IUserRepository _userRepo;

        public WalletService(IWalletRepository walletRepo, IPersonRepository personRepo, IUserRepository userRepo)
        {
            _walletRepo = walletRepo;
            _personRepo = personRepo;
            _userRepo = userRepo;
        }

        public WalletDto GetMyWallet(long personId)
        {
            EnsureRoleByPersonId(personId, UserRole.Tourist);

            var wallet = _walletRepo.GetByPersonId(personId) ?? _walletRepo.Create(new Wallet(personId));
            return new WalletDto { PersonId = personId, BalanceAc = wallet.BalanceAc };
        }

        public WalletDto TopUp(long adminPersonId, long touristUserId, int amountAc)
        {
            EnsureRoleByPersonId(adminPersonId, UserRole.Administrator);

            if (amountAc <= 0) throw new ArgumentException("Amount must be > 0.");

            var touristPerson = _personRepo.GetByUserId(touristUserId);
            if (touristPerson == null) throw new KeyNotFoundException("Tourist person not found.");

            EnsureRoleByPersonId(touristPerson.Id, UserRole.Tourist);

            var wallet = _walletRepo.GetByPersonId(touristPerson.Id) ?? _walletRepo.Create(new Wallet(touristPerson.Id));
            wallet.AddAc(amountAc);
            _walletRepo.Update(wallet);

            return new WalletDto { PersonId = touristPerson.Id, BalanceAc = wallet.BalanceAc };
        }

        private void EnsureRoleByPersonId(long personId, UserRole role)
        {
            var person = _personRepo.Get(personId) ?? throw new KeyNotFoundException("Person not found.");
            var user = _userRepo.Get(person.UserId) ?? throw new KeyNotFoundException("User not found.");
            if (user.Role != role) throw new UnauthorizedAccessException("Not allowed.");
        }
    }
}
