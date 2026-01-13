using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class InternalTouristXPAndLevelService : IInternalTouristXPAndLevelSerive
{
    private readonly ITouristRepository _touristRepository;

    public InternalTouristXPAndLevelService(ITouristRepository touristRepository)
    {
        _touristRepository = touristRepository;
    }

    public void AddExperience(long touristId, int xp)
    {
        var tourist = _touristRepository.Get(touristId);
        if (tourist == null)
            throw new KeyNotFoundException($"Tourist with id {touristId} not found.");

        tourist.IncreaseXP(xp);
        _touristRepository.Update(tourist);
    }
    public int GetLevel(long touristId)
    {
        var tourist = _touristRepository.Get(touristId);
        if (tourist == null)
            throw new KeyNotFoundException($"Tourist with id {touristId} not found.");
        return tourist.Level;
    }
}
