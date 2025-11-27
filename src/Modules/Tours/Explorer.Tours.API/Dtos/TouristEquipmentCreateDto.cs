using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TouristEquipmentCreateDto
{
    public long TouristId { get; set; }
    public long EquipmentId { get; set; }
}