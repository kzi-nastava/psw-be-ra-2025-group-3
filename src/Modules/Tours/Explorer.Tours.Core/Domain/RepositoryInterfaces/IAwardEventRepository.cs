using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IAwardEventRepository
    {
        PagedResult<AwardEvent> GetPaged(int page, int pageSize);

        AwardEvent Get(long id);
        AwardEvent Create(AwardEvent awardEvent);
        AwardEvent Update(AwardEvent awardEvent);
        void Delete(long id);
        bool ExistsForYear(int year, long? excludeId = null);
    }
}