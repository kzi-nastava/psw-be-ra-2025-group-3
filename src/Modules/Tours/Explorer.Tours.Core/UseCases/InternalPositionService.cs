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

    public PositionDto GetByTouristId(long touristId)
    {
        var position = _positionRepository.GetByTouristId(touristId);
        return _mapper.Map<PositionDto>(position);
    }

    public List<PositionDto> GetAll()
    {
        var positions = _positionRepository.GetAll();
        return _mapper.Map<List<PositionDto>>(positions);
    }
}