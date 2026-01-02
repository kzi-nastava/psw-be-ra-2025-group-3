using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class InternalPositionService : IInternalPositionService
{
    private readonly IPositionRepository _positionRepository;
    private readonly IMapper _mapper;

    public InternalPositionService(IPositionRepository positionRepository, IMapper mapper)
    {
        _positionRepository = positionRepository;
        _mapper = mapper;
    }

    public PositionDto? GetByTouristId(long touristId)
    {
        var position = _positionRepository.GetByTouristId(touristId);
        if (position == null)
            return null;

        return _mapper.Map<PositionDto>(position);
    }
}