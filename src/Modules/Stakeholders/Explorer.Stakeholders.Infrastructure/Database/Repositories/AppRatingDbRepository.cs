using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class AppRatingDbRepository : IAppRatingRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<AppRating> _dbSet;

        public AppRatingDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<AppRating>();
        }

        public AppRating Create(AppRating entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public AppRating? GetByUserId(long userId)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(r => r.UserId == userId);
        }

        public AppRating Update(AppRating entity)
        {
            try
            {
                _dbSet.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }

        public void Delete(AppRating entity)
        {
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public PagedResult<AppRating> GetPaged(int page, int pageSize)
        {
            var query = _dbSet.AsNoTracking().OrderByDescending(r => r.CreatedAt);

            var totalCount = query.Count();
            var results = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<AppRating>(results, totalCount);
        }
    }
}
