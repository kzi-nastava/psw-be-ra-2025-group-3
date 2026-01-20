using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class WelcomeBonusDbRepository : IWelcomeBonusRepository
{
    private readonly StakeholdersContext _context;
    
    public WelcomeBonusDbRepository(StakeholdersContext context) => _context = context;

    public WelcomeBonus? GetByPersonId(long personId)
        => _context.WelcomeBonuses.SingleOrDefault(wb => wb.PersonId == personId);

    public WelcomeBonus Create(WelcomeBonus bonus)
    {
        _context.WelcomeBonuses.Add(bonus);
        _context.SaveChanges();
        return bonus;
    }

    public WelcomeBonus Update(WelcomeBonus bonus)
    {
        _context.WelcomeBonuses.Update(bonus);
        _context.SaveChanges();
        return bonus;
    }

    public bool ExistsForPerson(long personId)
        => _context.WelcomeBonuses.Any(wb => wb.PersonId == personId);
}
