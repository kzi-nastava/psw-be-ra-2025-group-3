using AutoMapper;
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

        // Glavni flow: "Oceni aplikaciju" - automatski Create ili Update
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
                // Ako već ima ocenu, automatski uradi UPDATE (korisnik ne zna da ima ocenu)
                _mapper.Map(entity, rating);
                rating.Validate();
                rating.UpdatedAt = DateTime.Now;
                _repository.Update(rating);
            }

            return MapToDto(rating);
        }

        // Explicit edit: "Izmeni ocenu" - korisnik SVESNO menja postojeću ocenu
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

        public List<AppRatingResponseDto> GetAllRatings()
        {
            var ratings = _repository.GetAll();
            return ratings.Select(MapToDto).ToList();
        }

        private AppRatingResponseDto MapToDto(AppRating entity)
        {
            var rating = _mapper.Map<AppRatingResponseDto>(entity);
            var user = _userRepository.GetById(entity.UserId);
            rating.Username = user?.Username ?? string.Empty;
            return rating;
        }
    }
}
