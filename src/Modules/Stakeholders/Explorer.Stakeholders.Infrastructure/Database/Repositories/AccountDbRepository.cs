using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class AccountDbRepository : IAccountRepository
{
    private readonly StakeholdersContext _context;

    public AccountDbRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public bool Exists(string username)
    {
        return _context.Accounts.Any(a => a.Username == username);
    }

    public Account Create(Account account)
    {
        _context.Accounts.Add(account);
        _context.SaveChanges();
        return account;
    }

    public Account? Get(long id)
    {
        return _context.Accounts.Find(id);
    }

    public Account? GetByUsername(string username)
    {
        return _context.Accounts.FirstOrDefault(a => a.Username == username);
    }

    public List<Account> GetAll()
    {
        return _context.Accounts.ToList();
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
        _context.SaveChanges();
    }
}
