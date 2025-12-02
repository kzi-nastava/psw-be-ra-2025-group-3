using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Administration;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public EquipmentService(IEquipmentRepository repository, IMapper mapper)
    {
        _equipmentRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<EquipmentDto> GetPaged(int page, int pageSize)
    {
        var result = _equipmentRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<EquipmentDto>).ToList();
        return new PagedResult<EquipmentDto>(items, result.TotalCount);
    }

    // DODATO
    public List<EquipmentDto> GetAll()
    {
        var equipment = _equipmentRepository.GetAll();
        return equipment.Select(_mapper.Map<EquipmentDto>).ToList();
    }

    public EquipmentDto Create(EquipmentDto entity)
    {
        var result = _equipmentRepository.Create(_mapper.Map<Equipment>(entity));
        return _mapper.Map<EquipmentDto>(result);
    }

    public EquipmentDto Update(EquipmentDto entity)
    {
        var result = _equipmentRepository.Update(_mapper.Map<Equipment>(entity));
        return _mapper.Map<EquipmentDto>(result);
    }

    public void Delete(long id)
    {
        _equipmentRepository.Delete(id);
    }
}