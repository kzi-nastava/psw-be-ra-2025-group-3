using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IBundleRepository
    {
        Bundle Create(Bundle bundle);
        Bundle Update(Bundle bundle);
        void Delete(long id);
        Bundle? GetById(long id);
        List<Bundle> GetByAuthorId(long authorId);
        List<Bundle> GetPublished();
    }
}