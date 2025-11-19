using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITouristEquipmentRepository
{
    List<TouristEquipment> GetByTouristId(long touristId);
    TouristEquipment Create(TouristEquipment touristEquipment);
    void Delete(long touristId, long equipmentId);
    TouristEquipment GetByTouristAndEquipment(long touristId, long equipmentId);
}