using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubJoinRequestRepository : IClubJoinRequestRepository
    {
        private readonly StakeholdersContext _context;

        public ClubJoinRequestRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public ClubJoinRequest Get(long id)
        {
            var request = _context.ClubJoinRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) throw new KeyNotFoundException("Club join request not found.");
            return request;
        }

        public ClubJoinRequest GetByTouristAndClub(long touristId, long clubId)
        {
            return _context.ClubJoinRequests.FirstOrDefault(r => r.TouristId == touristId && r.ClubId == clubId);
        }

        public ClubJoinRequest Create(ClubJoinRequest request)
        {
            _context.ClubJoinRequests.Add(request);
            _context.SaveChanges();
            return request;
        }

        public List<ClubJoinRequest> GetByClub(long clubId)
        {
            return _context.ClubJoinRequests.Where(r => r.ClubId == clubId).ToList();
        }

        public void Delete(long id)
        {
            var request = Get(id);
            _context.ClubJoinRequests.Remove(request);
            _context.SaveChanges();
        }
    }
}