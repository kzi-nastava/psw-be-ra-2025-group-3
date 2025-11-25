using System.Linq;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class PositionDbRepository : IPositionRepository
    {
        private readonly ToursContext _context;

        public PositionDbRepository(ToursContext context)
        {
            _context = context;
        }

        public Position Create(Position position)
        {
            _context.Positions.Add(position);
            _context.SaveChanges();
            return position;
        }

        public Position Update(Position position)
        {
            _context.Positions.Update(position);
            _context.SaveChanges();
            return position;
        }

        public Position? GetByTouristId(long touristId)
        {
            return _context.Positions
                .FirstOrDefault(p => p.TouristId == touristId);
        }

        public bool Exists(long touristId)
        {
            return _context.Positions.Any(p => p.TouristId == touristId);
        }
    }
}
