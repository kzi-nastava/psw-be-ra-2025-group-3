using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public interface IAwardEventService
    {      
        PagedResult<AwardEventDto> GetPaged(int page, int pageSize);
        AwardEventDto Get(long id);
        AwardEventDto Create(AwardEventCreateDto awardEventDto);
        AwardEventDto Update(AwardEventUpdateDto awardEventDto);
        void Delete(long id);
    }
}
