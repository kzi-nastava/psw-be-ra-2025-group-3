using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IEquipmentRepository
{
    PagedResult<Equipment> GetPaged(int page, int pageSize);
    List<Equipment> GetAll(); // DODATO
    Equipment Create(Equipment map);
    Equipment Update(Equipment map);
    void Delete(long id);
    Equipment Get(long id);
}