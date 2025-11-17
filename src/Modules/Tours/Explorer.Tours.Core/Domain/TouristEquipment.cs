using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TouristEquipment      // many-to-many tabela, ne nasleđuje Entity 
{
    public long TouristId { get; init; }
    public long EquipmentId { get; init; }
    public Equipment Equipment { get; init; }  

    public TouristEquipment() { }

    public TouristEquipment(long touristId, long equipmentId)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId.");
        if (equipmentId == 0) throw new ArgumentException("Invalid EquipmentId.");

        TouristId = touristId;
        EquipmentId = equipmentId;
    }
}