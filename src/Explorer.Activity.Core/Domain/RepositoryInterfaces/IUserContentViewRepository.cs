using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Activity.Core.Domain.RepositoryInterfaces
{
    public interface IUserContentViewRepository
    {
        void Add(UserContentView view);
        List<UserContentView> GetByUser(int userId);
        List<long> GetMostViewedBlogIdsForUser(int userId, int take);
    }
}
