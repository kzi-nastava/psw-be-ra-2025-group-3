using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;

namespace Explorer.Tours.Core.UseCases.Authoring
{
    public class BundleService : IBundleService, IInternalBundleService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public BundleService(
            IBundleRepository bundleRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public BundleDto Create(BundleCreateDto bundleDto, long authorId)
        {
            foreach (var tourId in bundleDto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour == null)
                    throw new NotFoundException($"Tour with id {tourId} not found.");
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
            }

            var bundle = new Bundle(
                bundleDto.Name,
                bundleDto.Price,
                authorId,
                bundleDto.TourIds
            );

            var result = _bundleRepository.Create(bundle);
            return _mapper.Map<BundleDto>(result);
        }

        public BundleDto Update(BundleUpdateDto bundleDto, long authorId)
        {
            var bundle = _bundleRepository.GetById(bundleDto.Id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {bundleDto.Id} not found.");
            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("You can only update your own bundles.");

            foreach (var tourId in bundleDto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour == null)
                    throw new NotFoundException($"Tour with id {tourId} not found.");
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
            }

            bundle.Update(bundleDto.Name, bundleDto.Price, bundleDto.TourIds);

            var result = _bundleRepository.Update(bundle);
            return _mapper.Map<BundleDto>(result);
        }

        public void Delete(long id, long authorId)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {id} not found.");
            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("You can only delete your own bundles.");
            if (bundle.Status == BundleStatus.Published)
                throw new InvalidOperationException("Published bundles cannot be deleted. Archive them instead.");

            _bundleRepository.Delete(id);
        }

        public BundleWithToursDto GetById(long id)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {id} not found.");

            var tours = new List<TourDto>();
            decimal totalToursPrice = 0;

            foreach (var tourId in bundle.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour != null)
                {
                    tours.Add(_mapper.Map<TourDto>(tour));
                    totalToursPrice += tour.Price;
                }
            }

            return new BundleWithToursDto
            {
                Id = bundle.Id,
                Name = bundle.Name,
                Price = bundle.Price,
                Status = (int)bundle.Status,
                AuthorId = bundle.AuthorId,
                Tours = tours,
                TotalToursPrice = totalToursPrice,
                CreatedAt = bundle.CreatedAt
            };
        }

        public List<BundleDto> GetByAuthorId(long authorId)
        {
            var bundles = _bundleRepository.GetByAuthorId(authorId);
            return bundles.Select(_mapper.Map<BundleDto>).ToList();
        }

        public BundleDto Publish(long id, long authorId)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {id} not found.");
            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("You can only publish your own bundles.");

            int publishedCount = 0;
            foreach (var tourId in bundle.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour != null && tour.Status == TourStatus.Published)
                {
                    publishedCount++;
                }
            }

            if (publishedCount < 2)
            {
                throw new InvalidOperationException(
                    "Bundle must contain at least 2 published tours to be published.");
            }

            bundle.Publish();
            var result = _bundleRepository.Update(bundle);
            return _mapper.Map<BundleDto>(result);
        }

        public BundleDto Archive(long id, long authorId)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {id} not found.");
            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("You can only archive your own bundles.");

            bundle.Archive();
            var result = _bundleRepository.Update(bundle);
            return _mapper.Map<BundleDto>(result);
        }

        public List<BundleDto> GetPublished()
        {
            var bundles = _bundleRepository.GetPublished();
            return bundles.Select(_mapper.Map<BundleDto>).ToList();
        }
        // Internal metoda za Payments modul
        BundleDto IInternalBundleService.GetById(long id)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new NotFoundException($"Bundle with id {id} not found.");

            return _mapper.Map<BundleDto>(bundle);
        }
    }
}