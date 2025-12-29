using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IDiaryRepository
    {
        Diary Add(Diary diary);
        Diary Update(Diary diary);
        void Delete(Diary diary);

        Diary GetById(long id);
        List<Diary> GetByTourist(int touristId);
    }
}
