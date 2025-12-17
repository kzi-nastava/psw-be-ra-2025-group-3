using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface ITouristEquipmentService
{
    List<EquipmentWithOwnershipDto> GetAllEquipmentWithOwnership(long touristId);
    List<EquipmentWithOwnershipDto> GetTouristEquipment(long touristId);
    EquipmentWithOwnershipDto AddEquipmentToTourist(long touristId, long equipmentId);
    void DeleteEquipmentFromTourist(long touristId, long equipmentId);
}
