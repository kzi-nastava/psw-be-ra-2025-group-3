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
    private readonly IFacilityRepository _repository;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public FacilityDto Create(FacilityDto dto)
    {
        var facility = _mapper.Map<Facility>(dto);
        var created = _repository.Create(facility);
        return _mapper.Map<FacilityDto>(created);
    }

    public FacilityDto Update(FacilityDto dto)
    {
        var facility = _mapper.Map<Facility>(dto);
        var updated = _repository.Update(facility);
        return _mapper.Map<FacilityDto>(updated);
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }

    public FacilityDto Get(int id)
    {
        var facility = _repository.Get(id);
        return _mapper.Map<FacilityDto>(facility);
    }

    public List<FacilityDto> GetAll()
    {
        var facilities = _repository.GetAll();
        return _mapper.Map<List<FacilityDto>>(facilities);
    }
}
