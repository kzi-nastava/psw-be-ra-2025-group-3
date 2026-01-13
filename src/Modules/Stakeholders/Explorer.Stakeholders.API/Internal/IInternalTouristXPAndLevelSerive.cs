namespace Explorer.Stakeholders.API.Internal;

public interface IInternalTouristXPAndLevelSerive
{
    void AddExperience(long touristId, int xp);
    int GetLevel(long touristId);
}