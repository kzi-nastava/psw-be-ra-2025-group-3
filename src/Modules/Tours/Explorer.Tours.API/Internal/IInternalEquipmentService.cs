using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Internal;

public interface IInternalEquipmentService
{
    List<EquipmentDto> GetAll();
    EquipmentDto Get(long id);
}
