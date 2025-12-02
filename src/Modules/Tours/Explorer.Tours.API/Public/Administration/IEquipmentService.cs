using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public.Administration;

public interface IEquipmentService
{
    PagedResult<EquipmentDto> GetPaged(int page, int pageSize);
    List<EquipmentDto> GetAll(); // DODATO
    EquipmentDto Create(EquipmentDto equipment);
    EquipmentDto Update(EquipmentDto equipment);
    void Delete(long id);
}