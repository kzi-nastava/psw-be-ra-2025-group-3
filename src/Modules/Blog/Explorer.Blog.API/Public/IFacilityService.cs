using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IFacilityService
{
    FacilityDto Create(FacilityDto facility);
    FacilityDto Update(FacilityDto facility);
    void Delete(int id);
    FacilityDto Get(int id);
    List<FacilityDto> GetAll();
}