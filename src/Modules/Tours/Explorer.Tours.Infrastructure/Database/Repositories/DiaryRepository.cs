using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces; // ili gde ti je IDiaryRepository
using Explorer.Tours.Infrastructure.Database;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class DiaryDbRepository : IDiaryRepository
    {
        private readonly ToursContext _context;

        public DiaryDbRepository(ToursContext context)
        {
            _context = context;
        }

        public Diary Add(Diary diary)
        {
            _context.Diaries.Add(diary);
            _context.SaveChanges();
            return diary;
        }

        public Diary GetById(long id)
        {
            var diary = _context.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null) throw new KeyNotFoundException();
            return diary;
        }

        public List<Diary> GetByTourist(int touristId)
        {
            return _context.Diaries
                .Where(d => d.TouristId == touristId)
                .ToList();
        }

        public Diary Update(Diary diary)
        {
            _context.Diaries.Update(diary);
            _context.SaveChanges();
            return diary;
        }

        public void Delete(Diary diary)
        {
            _context.Diaries.Remove(diary);
            _context.SaveChanges();
        }
    }
}
