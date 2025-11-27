using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        PagedResult<Club> GetPaged(int page, int pageSize);
        Club Get(long id);
        Club Create(Club club);
        Club Update(Club club);
        void Delete(long id);
        PagedResult<Club> GetByOwnerId(long ownerId, int page, int pageSize);
        ClubImage CreateImageDirectly(long clubId, string imageUrl);
    }
}
