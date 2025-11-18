using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases; 
using Explorer.Tours.Infrastructure.Database; 

namespace Explorer.Tours.Infrastructure.Repositories
{
    public class AwardEventRepository : IAwardEventRepository
    {
        private readonly ToursContext _dbContext;

        public AwardEventRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedResult<AwardEvent> GetPaged(int page, int pageSize)
        {
            var query = _dbContext.AwardEvents.AsQueryable();      
            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<AwardEvent>(items, totalCount);
        }

        public AwardEvent Get(long id)
        {
            return _dbContext.AwardEvents.Find(id);
        }

        public AwardEvent Create(AwardEvent awardEvent)
        {
            _dbContext.AwardEvents.Add(awardEvent);
            _dbContext.SaveChanges();
            return awardEvent;
        }

        public AwardEvent Update(AwardEvent awardEvent)
        {
            _dbContext.AwardEvents.Update(awardEvent);
            _dbContext.SaveChanges();
            return awardEvent;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            if (entity != null)
            {
                _dbContext.AwardEvents.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public bool ExistsForYear(int year, long? excludeId = null)
        {
            var query = _dbContext.AwardEvents.Where(ae => ae.Year == year);

            if (excludeId.HasValue)
            {
                query = query.Where(ae => ae.Id != excludeId.Value);
            }
            return query.Any();
        }
    }
}
