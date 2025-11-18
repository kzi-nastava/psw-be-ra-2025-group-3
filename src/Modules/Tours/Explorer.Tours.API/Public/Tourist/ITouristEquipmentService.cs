using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristEquipmentService
{
    // Turista vidi svu opremu iz sistema sa oznakom šta poseduje
    List<EquipmentWithOwnershipDto> GetAllEquipmentWithOwnership(long touristId);

    // Turista vidi samo opremu koju poseduje
    List<EquipmentWithOwnershipDto> GetTouristEquipment(long touristId);

    // Turista dodaje opremu (čekira)
    EquipmentWithOwnershipDto AddEquipmentToTourist(TouristEquipmentCreateDto touristEquipment);

    // Turista uklanja opremu (odčekira)
    void DeleteEquipmentFromTourist(long touristId, long equipmentId);
}
