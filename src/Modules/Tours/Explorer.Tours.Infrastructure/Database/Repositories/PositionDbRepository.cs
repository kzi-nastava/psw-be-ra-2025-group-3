using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            // OSIGURAVAMO DA POSTOJI SAMO JEDNA POZICIJA PO TURISTI
            // Obriši sve postojeće pozicije za ovog turistu pre kreiranja nove
            var existingPositions = _context.Positions
                .Where(p => p.TouristId == position.TouristId)
                .ToList();
            
            if (existingPositions.Any())
            {
                _context.Positions.RemoveRange(existingPositions);
            }
            
            _context.Positions.Add(position);
            _context.SaveChanges();
            return position;
        }

        public Position Update(Position position)
        {
            // OSIGURAVAMO DA POSTOJI SAMO JEDNA POZICIJA PO TURISTI
            // Obriši sve ostale pozicije za ovog turistu (osim trenutne koja se ažurira)
            var otherPositions = _context.Positions
                .Where(p => p.TouristId == position.TouristId && p.Id != position.Id)
                .ToList();
            
            if (otherPositions.Any())
            {
                _context.Positions.RemoveRange(otherPositions);
            }
            
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

        public List<Position> GetAll()
        {
            return _context.Positions.ToList();
        }
    }
}
