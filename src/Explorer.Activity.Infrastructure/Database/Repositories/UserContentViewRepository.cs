using Explorer.Activity.Core.Domain;
using Explorer.Activity.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Activity.Infrastructure.Database.Repositories
{
    public class UserContentViewRepository : IUserContentViewRepository
    {
        private readonly ActivityContext _context;

        public UserContentViewRepository(ActivityContext context)
        {
            _context = context;
        }

        public void Add(UserContentView view)
        {
            _context.UserContentViews.Add(view);
            _context.SaveChanges();
        }

        public List<UserContentView> GetByUser(int userId)
        {
            return _context.UserContentViews
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.ViewedAt)
                .ToList();
        }

        public List<long> GetMostViewedBlogIdsForUser(int userId, int take)
        {
            return _context.UserContentViews
                .Where(v => v.UserId == userId && v.ContentType == ContentType.Blog)
                .GroupBy(v => v.ContentId)
                .OrderByDescending(g => g.Count())
                .Take(take)
                .Select(g => g.Key)
                .ToList();
        }
    }
}
