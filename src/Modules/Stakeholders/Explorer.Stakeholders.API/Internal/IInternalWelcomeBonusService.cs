using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalWelcomeBonusService
{
    WelcomeBonusDto? GetActiveDiscountBonus(long personId);
    void MarkBonusAsUsed(long personId);
}
