using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class AppRatingService : IAppRatingService
    {
        private readonly IAppRatingRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AppRatingService(IAppRatingRepository repository, IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public AppRatingResponseDto CreateRating(long userId, AppRatingRequestDto entity)
        {
            var rating = _repository.GetByUserId(userId);

            if (rating == null)
            {
                var newRating = new AppRating(userId, entity.Rating, entity.Comment);
                _repository.Create(newRating);
                rating = newRating;
            }
            else
            {
                _mapper.Map(entity, rating);
                rating.Validate();
                rating.UpdatedAt = DateTime.Now;
                _repository.Update(rating);
            }

            return MapToDto(rating);
        }

        public AppRatingResponseDto UpdateRating(long userId, AppRatingRequestDto entity)
        {
            var rating = _repository.GetByUserId(userId);
            if (rating == null)
            {
                throw new KeyNotFoundException($"Rating for user {userId} not found. Cannot update non-existing rating.");
            }

            _mapper.Map(entity, rating);
            rating.Validate();
            rating.UpdatedAt = DateTime.Now;
            _repository.Update(rating);

            return MapToDto(rating);
        }

        public void DeleteRating(long userId)
        {
            var rating = _repository.GetByUserId(userId);
            if (rating == null)
            {
                throw new KeyNotFoundException($"Rating for user {userId} not found.");
            }

            _repository.Delete(rating);
        }

        public AppRatingResponseDto? GetMyRating(long userId)
        {
            var rating = _repository.GetByUserId(userId);
            return rating == null ? null : MapToDto(rating);
        }

        public PagedResult<AppRatingResponseDto> GetPaged(int page, int pageSize)
        {
            var result = _repository.GetPaged(page, pageSize);
            var items = result.Results.Select(MapToDto).ToList();
            return new PagedResult<AppRatingResponseDto>(items, result.TotalCount);
        }

        private AppRatingResponseDto MapToDto(AppRating entity)
        {
            var rating = _mapper.Map<AppRatingResponseDto>(entity);
            var user = _userRepository.Get(entity.UserId);
            rating.Username = user?.Username ?? string.Empty;
            return rating;
        }
    }
}
