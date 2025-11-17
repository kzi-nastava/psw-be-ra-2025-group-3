using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface IPreferenceService
{
    PreferenceDto Create(PreferenceCreateDto preferenceDto, long touristId);
    PreferenceDto Update(PreferenceUpdateDto preferenceDto, long touristId);
    void Delete(long touristId);
    PreferenceDto GetByTouristId(long touristId);
}