using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IKeyPointRepository
    {
        PagedResult<KeyPoint> GetPaged(int page, int pageSize);
        KeyPoint Create(KeyPoint keyPoint);
        KeyPoint Update(KeyPoint keyPoint);
        void Delete(long id);
        KeyPoint Get(long id);
    }
}