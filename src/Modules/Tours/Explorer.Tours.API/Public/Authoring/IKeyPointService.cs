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
        PagedResult<KeyPointDto> GetPaged(long tourId, int page, int pageSize);
        KeyPointDto Create(KeyPointDto keyPoint, long authorId);
        KeyPointDto Update(KeyPointDto keyPoint, long authorId);
        void Delete(long id, long authorId);
        KeyPointDto GetById(long id);
    }
}