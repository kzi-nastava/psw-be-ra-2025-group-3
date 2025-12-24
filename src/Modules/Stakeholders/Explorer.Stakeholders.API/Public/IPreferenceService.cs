using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IPreferenceService
{
    PreferenceDto Create(PreferenceCreateDto preferenceDto, long touristId);
    PreferenceDto Update(PreferenceUpdateDto preferenceDto, long touristId);
    void Delete(long touristId);
    PreferenceDto? GetByTouristId(long touristId);

    List<RecommendedTourDto> GetRecommendedTours(long touristId); // tour recommendations
}
