using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

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
        if (facility == null) throw new KeyNotFoundException("Facility not found.");

        _mapper.Map(dto, facility);   // ovde će se pozvati Update() kroz AfterMap u profilu
        _facilityRepository.Update(facility);

        return _mapper.Map<FacilityDto>(facility);
    }

    public void Delete(long id)
    {
        _facilityRepository.Delete(id);
    }
}
