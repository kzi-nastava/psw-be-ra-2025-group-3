using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IWelcomeBonusService
{
    WelcomeBonusDto GetWelcomeBonus(long personId);
    WelcomeBonusDto CreateWelcomeBonus(long personId);
}
