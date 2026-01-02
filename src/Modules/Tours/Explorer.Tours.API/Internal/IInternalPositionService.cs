using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Internal;

public interface IInternalPositionService
{
    PositionDto? GetByTouristId(long touristId);
}