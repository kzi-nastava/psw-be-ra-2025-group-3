using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class BundleDbRepository : IBundleRepository
    {
        private readonly ToursContext _context;

        public BundleDbRepository(ToursContext context)
        {
            _context = context;
        }

        public Bundle Create(Bundle bundle)
        {
            _context.Bundles.Add(bundle);
            _context.SaveChanges();
            return bundle;
        }

        public Bundle Update(Bundle bundle)
        {
            _context.Bundles.Update(bundle);
            _context.SaveChanges();
            return bundle;
        }

        public void Delete(long id)
        {
            var bundle = _context.Bundles.Find(id);
            if (bundle != null)
            {
                _context.Bundles.Remove(bundle);
                _context.SaveChanges();
            }
        }

        public Bundle? GetById(long id)
        {
            return _context.Bundles.Find(id);
        }

        public List<Bundle> GetByAuthorId(long authorId)
        {
            return _context.Bundles
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }

        public List<Bundle> GetPublished()
        {
            return _context.Bundles
                .Where(b => b.Status == BundleStatus.Published)
                .ToList();
        }
    }
}