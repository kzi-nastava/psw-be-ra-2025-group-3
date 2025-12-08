using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class KeyPointDbRepository : IKeyPointRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<KeyPoint> _dbSet;

        public KeyPointDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<KeyPoint>();
        }

        public PagedResult<KeyPoint> GetPaged(long tourId, int page, int pageSize)
        {
            // filtriramo po turi
            var query = _dbSet
                .Where(kp => kp.TourId == tourId)
                .OrderBy(kp => kp.Id);

            // ukupno elemenata za tu turu
            var totalCount = query.Count();

            // stranica
            var items = query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<KeyPoint>(items, totalCount);
        }

        public KeyPoint Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public KeyPoint Create(KeyPoint entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public KeyPoint Update(KeyPoint entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }
    }
}