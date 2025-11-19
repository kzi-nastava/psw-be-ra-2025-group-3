using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _repository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PagedResult<ClubDto> GetPaged(int page, int pageSize)
        {
            var clubs = _repository.GetPaged(page, pageSize);
            var clubsDtos = _mapper.Map<List<ClubDto>>(clubs.Results);
            return new PagedResult<ClubDto>(clubsDtos, clubs.TotalCount);
        }

        public ClubDto Get(long id)
        {
            var club = _repository.Get(id);
            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            return _mapper.Map<ClubDto>(club);
        }

        public ClubDto Create(ClubCreateDto clubDto, long userId)
        {
            if (string.IsNullOrWhiteSpace(clubDto.Name))
                throw new ArgumentException("Club name is required.");

            if (string.IsNullOrWhiteSpace(clubDto.Description))
                throw new ArgumentException("Club description is required.");

            if (string.IsNullOrWhiteSpace(clubDto.FeaturedImageUrl))
                throw new ArgumentException("Featured image URL is required.");

            var club = new Club(clubDto.Name, clubDto.Description, userId);
            club = _repository.Create(club);

            var featuredImage = _repository.CreateImageDirectly(club.Id, clubDto.FeaturedImageUrl);
            
            club.SetFeaturedImage(featuredImage.Id);
            club = _repository.Update(club);

            if (clubDto.GalleryImageUrls != null && clubDto.GalleryImageUrls.Any())
            {
                foreach (var imageUrl in clubDto.GalleryImageUrls)
                {
                    var galleryImage = new ClubImage(club.Id, imageUrl);
                    club.AddGalleryImage(galleryImage);
                }
                club = _repository.Update(club);
            }
            club = _repository.Get(club.Id);
            return _mapper.Map<ClubDto>(club);
        }

        public ClubDto Update(long id, ClubUpdateDto clubDto, long userId)
        {
            var club = _repository.Get(id);
            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            if (club.OwnerId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this club.");

            if (!string.IsNullOrWhiteSpace(clubDto.Name) || !string.IsNullOrWhiteSpace(clubDto.Description))
            {
                club.Update(
                    clubDto.Name ?? club.Name,
                    clubDto.Description ?? club.Description
                );
            }
            
            if (clubDto.PromoteGalleryImageId.HasValue)
            {
                club.PromoteGalleryImageToFeatured(clubDto.PromoteGalleryImageId.Value);
                club = _repository.Update(club);
            }

            if (!string.IsNullOrWhiteSpace(clubDto.NewFeaturedImageUrl))
            {
                var newFeaturedImage = _repository.CreateImageDirectly(id, clubDto.NewFeaturedImageUrl);
                club.AddGalleryImage(newFeaturedImage);
                club = _repository.Update(club);
                var addedImage = club.Images.FirstOrDefault(i => i.ImageUrl == clubDto.NewFeaturedImageUrl);
                
                if (addedImage != null)
                {
                    club.SetFeaturedImage(newFeaturedImage.Id);
                    club = _repository.Update(club);
                }
            }

            if (clubDto.RemovedGalleryImageIds != null)
            {
                foreach (var imageId in clubDto.RemovedGalleryImageIds)
                {
                    club.RemoveGalleryImage(imageId);
                }
            }

            if (clubDto.NewGalleryImageUrls != null)
            {
                foreach (var imageUrl in clubDto.NewGalleryImageUrls)
                {
                    var galleryImage = new ClubImage(id, imageUrl);
                    club.AddGalleryImage(galleryImage);
                }
            }

            var updatedClub = _repository.Update(club);
            return _mapper.Map<ClubDto>(updatedClub);
        }

        public void Delete(long id, long userId)
        {
            var club = _repository.Get(id);
            if (club == null)
                throw new KeyNotFoundException($"Club with ID {id} not found.");

            if (club.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this club.");

            _repository.Delete(id);
        }

        public PagedResult<ClubDto> GetUserClubs(long userId, int page, int pageSize)
        {
            var clubs = _repository.GetByOwnerId(userId, page, pageSize);
            var clubDtos = _mapper.Map<List<ClubDto>>(clubs.Results);
            return new PagedResult<ClubDto>(clubDtos, clubs.TotalCount);
        }
    }
}
