using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;
    private readonly IMapper _mapper;

    public PositionService(IPositionRepository positionRepository, IMapper mapper)
    {
        _positionRepository = positionRepository;
        _mapper = mapper;
    }

    public PositionDto GetForTourist(long touristId)
    {
        var position = _positionRepository.GetByTouristId(touristId);

        if (position == null)
            return null!; // frontend zna da null znači “nema pozicije”

        var dto = _mapper.Map<PositionDto>(position);
        dto.TouristId = position.TouristId;
        return dto;
    }

    public void Update(long touristId, PositionDto dto)
    {
        // prvo proveri da li postoji turista (dodaj Exists metodu u repozitorijum)
        if (!_positionRepository.Exists(touristId))
            throw new Exception($"Tourist with id {touristId} does not exist.");

        var existing = _positionRepository.GetByTouristId(touristId);

        if (existing == null)
        {
            // create
            var pos = new Position(touristId, dto.Latitude, dto.Longitude);
            _positionRepository.Create(pos);
            return;
        }

        // update
        existing.Update(dto.Latitude, dto.Longitude);
        _positionRepository.Update(existing);
    }
}
