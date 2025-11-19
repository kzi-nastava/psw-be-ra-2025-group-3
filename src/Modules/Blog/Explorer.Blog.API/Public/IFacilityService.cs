using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;


using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IFacilityService
{
    FacilityDto Create(FacilityCreateDto dto);
    List<FacilityDto> GetAll();
    FacilityDto Update(long id, FacilityUpdateDto dto);
    void Delete(long id);
}