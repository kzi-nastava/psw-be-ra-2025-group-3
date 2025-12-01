using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Authoring
{
    public interface IKeyPointService
    {
        PagedResult<KeyPointDto> GetPaged(int page, int pageSize);
        KeyPointDto Create(KeyPointDto keyPoint);
        KeyPointDto Update(KeyPointDto keyPoint);
        void Delete(long id);
        KeyPointDto GetById(long id);
    }
}