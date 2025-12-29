using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IDiaryService
    {
        DiaryDto Create(DiaryCreateDto dto, int userId);
        List<DiaryDto> GetMyDiaries(int userId);
        DiaryDto Update(long id, DiaryCreateDto dto, int userId);
        void Delete(long id, int userId);

        // opciono:
        DiaryDto Archive(long id, int userId);
    }
}
