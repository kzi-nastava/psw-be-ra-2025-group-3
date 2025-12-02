using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public;

public interface IFacilityService
{
    FacilityDto Create(FacilityCreateDto dto);
    List<FacilityDto> GetAll();
    FacilityDto Update(long id, FacilityUpdateDto dto);
    void Delete(long id);
}