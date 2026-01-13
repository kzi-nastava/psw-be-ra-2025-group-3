using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring
{
    public interface IBundleService
    {
        BundleDto Create(BundleCreateDto bundleDto, long authorId);
        BundleDto Update(BundleUpdateDto bundleDto, long authorId);
        void Delete(long id, long authorId);
        BundleWithToursDto GetById(long id);
        List<BundleDto> GetByAuthorId(long authorId);
        BundleDto Publish(long id, long authorId);
        BundleDto Archive(long id, long authorId);
        List<BundleDto> GetPublished();
    }
}