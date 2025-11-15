using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases.Authoring
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accounts;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accounts, IMapper mapper)
        {
            _accounts = accounts;
            _mapper = mapper;
        }

        public AccountDto Create(AccountCreateDto dto)
        {
            if (_accounts.Exists(dto.Username))
                throw new EntityValidationException("Username already exists.");

            if (dto.Password.Length < 6)
                throw new EntityValidationException("Password must be at least 6 characters long.");

            if (!dto.Email.Contains("@"))
                throw new EntityValidationException("Invalid email format.");

            var role = Enum.Parse<AccountRole>(dto.Role, true);

            var account = new Account(dto.Username, dto.Password, dto.Email, role);

            _accounts.Create(account);

            
            return _mapper.Map<AccountDto>(account);
        }

        public List<AccountDto> GetAll()
        {
            var accounts = _accounts.GetAll();
            return accounts.Select(_mapper.Map<AccountDto>).ToList();
        }

        public AccountDto Get(int id)
        {
            var account = _accounts.Get(id);
            if (account == null)
                throw new KeyNotFoundException("Account not found.");

            return _mapper.Map<AccountDto>(account);
        }

        public void Block(int id)
        {
            var account = _accounts.Get(id);
            if (account == null)
                throw new KeyNotFoundException("Account not found.");

            if (account.Role == AccountRole.Admin)
                throw new EntityValidationException("Cannot block administrator accounts.");

            account.Block();
            _accounts.Update(account);
        }

        public void Unblock(int id)
        {
            var account = _accounts.Get(id);
            if (account == null)
                throw new KeyNotFoundException("Account not found.");

            account.Unblock();
            _accounts.Update(account);
        }
    }
}
