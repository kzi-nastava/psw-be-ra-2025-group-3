using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Administration;

public class EquipmentService : IEquipmentService
{
    private readonly ICrudRepository<Equipment> _crudRepository;
    private readonly IMapper _mapper;

    public EquipmentService(ICrudRepository<Equipment> repository, IMapper mapper)
    {
        _crudRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<EquipmentDto> GetPaged(int page, int pageSize)
    {
        var result = _crudRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<EquipmentDto>).ToList();
        return new PagedResult<EquipmentDto>(items, result.TotalCount);
    }

    public EquipmentDto Create(EquipmentDto entity)
    {
        var result = _crudRepository.Create(_mapper.Map<Equipment>(entity));
        return _mapper.Map<EquipmentDto>(result);
    }

    public EquipmentDto Update(EquipmentDto entity)
    {
        var result = _crudRepository.Update(_mapper.Map<Equipment>(entity));
        return _mapper.Map<EquipmentDto>(result);
    }

    public void Delete(long id)
    {
        _crudRepository.Delete(id);
    }
}