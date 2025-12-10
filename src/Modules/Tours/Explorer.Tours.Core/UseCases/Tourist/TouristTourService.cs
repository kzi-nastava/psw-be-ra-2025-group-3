using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public TouristTourService(ITourRepository tourRepository, ITourReviewRepository reviewRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public List<TourPreviewDto> GetPublishedTours()
    {
        var publishedTours = _tourRepository.GetPublished();
        var dtos = new List<TourPreviewDto>();

        foreach (var tour in publishedTours)
        {
            var dto = _mapper.Map<TourPreviewDto>(tour);

            if (tour.KeyPoints == null || !tour.KeyPoints.Any())
            {
                var tourWithKeyPoints = _tourRepository.GetByIdWithKeyPoints(tour.Id);
                if (tourWithKeyPoints != null && tourWithKeyPoints.KeyPoints.Any())
                {
                    var firstPoint = tourWithKeyPoints.KeyPoints.FirstOrDefault();
                    dto.FirstKeyPoint = _mapper.Map<KeyPointDto>(firstPoint);
                }
            }
            else
            {
                var firstPoint = tour.KeyPoints.FirstOrDefault();
                dto.FirstKeyPoint = _mapper.Map<KeyPointDto>(firstPoint);
            }

            var reviews = _reviewRepository.GetAllForTour(tour.Id);
            var reviewDtos = new List<TourReviewDto>();

            if (reviews != null && reviews.Any())
            {
                dto.AverageRating = reviews.Average(r => r.Rating);

                foreach (var review in reviews)
                {
                    var reviewDto = _mapper.Map<TourReviewDto>(review);
                    reviewDtos.Add(reviewDto);
                }
            }
            else
            {
                dto.AverageRating = 0;
            }

            dto.Reviews = reviewDtos;
            dtos.Add(dto);
        }

        return dtos;
    }
}