namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IWelcomeBonusRepository
{
    WelcomeBonus Create(WelcomeBonus bonus);
    WelcomeBonus? GetByPersonId(long personId);
    WelcomeBonus Update(WelcomeBonus bonus);
    bool ExistsForPerson(long personId);
}
