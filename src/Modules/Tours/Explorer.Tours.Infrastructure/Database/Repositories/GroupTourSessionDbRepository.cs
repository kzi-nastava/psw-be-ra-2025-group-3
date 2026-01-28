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
    public class GroupTourSessionDbRepository : IGroupTourSessionRepository
    {
        private readonly ToursContext _context;

        public GroupTourSessionDbRepository(ToursContext context)
        {
            _context = context;
        }

        public GroupTourSession Update(GroupTourSession session)
        {
            try
            {
                _context.GroupTourSessions.Update(session);
                _context.SaveChanges();
                return session;
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException(e.Message);
            }
        }

        public GroupTourSession? FindById(long id)
        {
            var session = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.Id == id)
                .FirstOrDefault();
            if (session == null)
                throw new KeyNotFoundException($"GroupTourSession with ID {id} not found.");
            
            return session;
        }

        public List<GroupTourSession> FindActiveByClubId(long clubId)
        {
            var sessions = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.ClubId == clubId && s.Status == GroupTourSessionStatus.Active)
                .ToList();
            return sessions;
        }

        public List<GroupTourSession> FindHighlightedByClubId(long clubId)
        {
            var sessions = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.ClubId == clubId && s.Status == GroupTourSessionStatus.Ended && s.IsHighlighted == true)
                .ToList();
            return sessions;
        }

        public List<GroupTourSession> FindSessionsForHighlightMarking(long clubId)
        {
            var sessions = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.ClubId == clubId && s.Status == GroupTourSessionStatus.Ended && s.IsHighlighted == null)
                .ToList();
            return sessions;
        }

        public List<GroupTourSession> FindByClubId(long clubId)
        {
            var sessions = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.ClubId == clubId)
                .ToList();
            return sessions;
        }

        public List<GroupTourSessionParticipant>? FindOtherGroupParticipantsByTouristId(long touristId)
        {
            var session = _context.GroupTourSessions
                .Include(s => s.Participants)
                .Where(s => s.Status == GroupTourSessionStatus.Active && s.Participants.Any(p => p.TouristId == touristId && p.LeftAt == null))
                .FirstOrDefault();
            
            // Ako nema aktivne group session, vrati null umesto da baca exception
            // Turista možda ima solo tour execution, što je validno
            if (session == null)
                return null;

            return session.Participants.Where(p => p.TouristId != touristId && p.LeftAt == null).ToList();
        }

        public GroupTourSession CreateGroupTourSession(GroupTourSession session)
        {
            _context.GroupTourSessions.Add(session);
            _context.SaveChanges();
            return session;
        }        

        public GroupTourSessionParticipant AddParticipant(GroupTourSessionParticipant participant)
        {
            _context.GroupTourSessionParticipants.Add(participant);            
            _context.SaveChanges();
            return participant;
        }
        public GroupTourSessionParticipant UpdateParticipant(GroupTourSessionParticipant participant)
        {
            _context.GroupTourSessionParticipants.Update(participant);
            _context.SaveChanges();
            return participant;
        }

        public GroupTourSessionParticipant FindParticipantByTourExecutionId(long tourExecutionId)
        {
            var participant = _context.GroupTourSessionParticipants.Where(p => p.TourExecutionId == tourExecutionId)
                .FirstOrDefault();

            if (participant == null)
                throw new KeyNotFoundException("Participant with the given TourExecutionId not found.");
            return participant;
        }
    }
}
