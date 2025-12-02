using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Authoring;

namespace Explorer.Tours.Core.UseCases.Authoring
{
    public class KeyPointService : IKeyPointService
    {
        private readonly IKeyPointRepository _keypointRepository;
        private readonly IMapper _mapper;

        public KeyPointService(IKeyPointRepository repository, IMapper mapper)
        {
            _keypointRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<KeyPointDto> GetPaged(int page, int pageSize)
        {
            var result = _keypointRepository.GetPaged(page, pageSize);

            var items = result.Results
                .Select(_mapper.Map<KeyPointDto>)
                .ToList();

            return new PagedResult<KeyPointDto>(items, result.TotalCount);
        }

        public KeyPointDto Create(KeyPointDto entity)
        {
            var created = _keypointRepository.Create(_mapper.Map<KeyPoint>(entity));
            return _mapper.Map<KeyPointDto>(created);
        }

        public KeyPointDto Update(KeyPointDto entity)
        {
            var updated = _keypointRepository.Update(_mapper.Map<KeyPoint>(entity));
            return _mapper.Map<KeyPointDto>(updated);
        }

        public void Delete(long id)
        {
            _keypointRepository.Delete(id);
        }

        public KeyPointDto GetById(long id)
        {
            var result = _keypointRepository.Get(id);
            return _mapper.Map<KeyPointDto>(result);
        }
    }
}