using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Tours.Core.UseCases;

public class InternalEquipmentService : IInternalEquipmentService
{
    private readonly IEquipmentService _equipmentService;

    public InternalEquipmentService(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    public List<EquipmentDto> GetAll()
    {
        return _equipmentService.GetPaged(0, 1000).Results.ToList();
    }

    public EquipmentDto Get(long id)
    {
        var allEquipment = _equipmentService.GetPaged(0, 1000).Results;
        return allEquipment.FirstOrDefault(e => e.Id == id);
    }
}
