using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;   // ⬅⬅⬅ OVO DODAJ

namespace Explorer.Blog.Core.UseCases.Administration;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository facilityRepository, IMapper mapper)
    {
        _facilityRepository = facilityRepository;
        _mapper = mapper;
    }

    public FacilityDto Create(FacilityCreateDto dto)
    {
        var facility = _mapper.Map<Facility>(dto);
        _facilityRepository.Create(facility);
        return _mapper.Map<FacilityDto>(facility);
    }

    public List<FacilityDto> GetAll()
    {
        var entities = _facilityRepository.GetAll();
        return entities.Select(f => _mapper.Map<FacilityDto>(f)).ToList();
    }

    public FacilityDto Update(long id, FacilityUpdateDto dto)
    {
        var facility = _facilityRepository.Get(id);

        if (facility == null)
            throw new NotFoundException("Facility not found.");   // ⬅⬅ OVO

        _mapper.Map(dto, facility);
        _facilityRepository.Update(facility);

        return _mapper.Map<FacilityDto>(facility);
    }

    public void Delete(long id)
    {
        var facility = _facilityRepository.Get(id);

        if (facility == null)
            throw new NotFoundException("Facility not found.");   // ⬅⬅ I OVO

        _facilityRepository.Delete(id);
    }
}
