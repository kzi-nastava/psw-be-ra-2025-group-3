using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Authoring
{
    public class KeyPointService : IKeyPointService
    {
        private readonly IKeyPointRepository _keyPointRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public KeyPointService(IKeyPointRepository keyPointRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _keyPointRepository = keyPointRepository;
            _mapper = mapper;
        }

        public PagedResult<KeyPointDto> GetPaged(long tourId, int page, int pageSize)
        {
            
            var result = _keyPointRepository.GetPaged(tourId, page, pageSize);

            var items = result.Results
                .Select(_mapper.Map<KeyPointDto>)
                .ToList();

            return new PagedResult<KeyPointDto>(items, result.TotalCount);
        }

        public KeyPointDto Create(KeyPointDto dto, long authorId)
        {
            // 1) Učitaj turu
            var tour = _tourRepository.GetById(dto.TourId);
            if (tour == null)
                throw new NotFoundException($"Tour with id {dto.TourId} not found.");

            // 2) Autorizacija – da li je ovaj autor vlasnik ture
            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only add key points to your own tours.");

            // 3) Kreiraj ključnu tačku
            var entity = _mapper.Map<KeyPoint>(dto);
            var created = _keyPointRepository.Create(entity);

            return _mapper.Map<KeyPointDto>(created);
        }

        public KeyPointDto Update(KeyPointDto dto, long authorId)
        {
            var existing = _keyPointRepository.Get(dto.Id);
            if (existing == null)
                throw new NotFoundException($"Key point with id {dto.Id} not found.");

            // Učitaj turu kojoj pripada
            var tour = _tourRepository.GetById(existing.TourId);
            if (tour == null)
                throw new NotFoundException($"Tour with id {existing.TourId} not found.");

            // Autorizacija – autor mora biti vlasnik ture
            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only update key points on your own tours.");

            // Domenska metoda radi validaciju i postavlja polja
            existing.Update(dto.Name,
                            dto.Description,
                            dto.ImageUrl,
                            dto.Secret,
                            dto.Latitude,
                            dto.Longitude);

            var updated = _keyPointRepository.Update(existing);
            return _mapper.Map<KeyPointDto>(updated);
        }

        public void Delete(long id, long authorId)
        {
            var existing = _keyPointRepository.Get(id);
            if (existing == null)
                throw new NotFoundException($"Key point with id {id} not found.");

            var tour = _tourRepository.GetById(existing.TourId);
            if (tour == null)
                throw new NotFoundException($"Tour with id {existing.TourId} not found.");

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only delete key points from your own tours.");

            _keyPointRepository.Delete(id);
        }

        public KeyPointDto GetById(long id)
        {
            var result = _keyPointRepository.Get(id);
            return _mapper.Map<KeyPointDto>(result);
        }
    }
}