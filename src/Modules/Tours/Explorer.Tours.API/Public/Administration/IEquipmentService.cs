using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IEquipmentService
{
    PagedResult<EquipmentDto> GetPaged(int page, int pageSize);
    EquipmentDto Create(EquipmentDto equipment);
    EquipmentDto Update(EquipmentDto equipment);
    void Delete(long id);
}