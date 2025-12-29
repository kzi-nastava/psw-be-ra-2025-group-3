using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _repo;
        private readonly IMapper _mapper;

        public DiaryService(IDiaryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public DiaryDto Create(DiaryCreateDto dto, int userId)
        {
            var entity = new Diary(dto.Title, dto.Country, dto.City, userId);
            var created = _repo.Add(entity);
            return _mapper.Map<DiaryDto>(created);
        }

        public List<DiaryDto> GetMyDiaries(int userId)
        {
            var diaries = _repo.GetByTourist(userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            return _mapper.Map<List<DiaryDto>>(diaries);
        }

        public DiaryDto Update(long id, DiaryCreateDto dto, int userId)
        {
            var diary = _repo.GetById(id);
            diary.Update(dto.Title, dto.Country, dto.City, userId);

            var updated = _repo.Update(diary);
            return _mapper.Map<DiaryDto>(updated);
        }

        public void Delete(long id, int userId)
        {
            var diary = _repo.GetById(id);
            diary.EnsureOwner(userId);

            _repo.Delete(diary);
        }

        public DiaryDto Archive(long id, int userId)
        {
            var diary = _repo.GetById(id);
            diary.Archive(userId);

            var updated = _repo.Update(diary);
            return _mapper.Map<DiaryDto>(updated);
        }
    }
}
