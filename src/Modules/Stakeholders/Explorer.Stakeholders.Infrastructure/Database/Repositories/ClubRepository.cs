using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly StakeholdersContext _dbContext;

        public ClubRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedResult<Club> GetPaged(int page, int pageSize)
        {
            var query = _dbContext.Clubs
                .Include(c => c.Images)
                .Include(c => c.FeaturedImage)
                .AsQueryable();

            var totalCount = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Club>(items, totalCount);
        }

        public Club Get(long id)
        {
            return _dbContext.Clubs
                .Include(c => c.Images)
                .Include(c => c.FeaturedImage)
                .FirstOrDefault(c => c.Id == id);
        }

        public Club Create(Club club)
        {
            _dbContext.Clubs.Add(club);
            _dbContext.SaveChanges();
            return club;
        }

        public Club Update(Club club)
        {
            _dbContext.Clubs.Update(club);
            _dbContext.SaveChanges();
            return club;
        }

        public void Delete(long id)
        {
            var club = _dbContext.Clubs
        .Include(c => c.Images)
        .Include(c => c.FeaturedImage)
        .FirstOrDefault(c => c.Id == id);

            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            club.SetFeaturedImage(null);
            _dbContext.SaveChanges();
            _dbContext.Clubs.Remove(club);
            _dbContext.SaveChanges();
        }

        public PagedResult<Club> GetByOwnerId(long ownerId, int page, int pageSize)
        {
            var query = _dbContext.Clubs
                .Include(c => c.Images)
                .Include(c => c.FeaturedImage)
                .Where(c => c.OwnerId == ownerId)
                .AsQueryable();

            var totalCount = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Club>(items, totalCount);
        }

        public ClubImage CreateImageDirectly(long clubId, string imageUrl)
        {
            var image = new ClubImage(clubId, imageUrl);
            _dbContext.ClubImages.Add(image);
            _dbContext.SaveChanges();
            return image;
        }
    }
}
