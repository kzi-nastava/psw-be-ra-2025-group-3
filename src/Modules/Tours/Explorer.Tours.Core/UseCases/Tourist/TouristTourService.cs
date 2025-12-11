using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourAccessService _access;
    private readonly IMapper _mapper;

    public TouristTourService(ITourRepository tourRepository, ITourReviewRepository reviewRepository, ITourAccessService access, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _reviewRepository = reviewRepository;
        _access = access;
        _mapper = mapper;
    }

    public List<TourPreviewDto> GetPublishedTours()
    {
        var publishedTours = _tourRepository.GetPublished();
        var result = new List<TourPreviewDto>();
      
        foreach (var tour in publishedTours)
        {
            var dto = _mapper.Map<TourPreviewDto>(tour);

            //
            // 1) LENGTH = DistanceInKm iz Tour entiteta
            //
            dto.Length = tour.DistanceInKm;

            //
            // 2) AverageDuration = avg(TimeInMinutes) iz JSON liste TourDurations
            //
            if (tour.TourDurations != null && tour.TourDurations.Any())
                dto.AverageDuration = tour.TourDurations.Average(td => td.TimeInMinutes);
            else
                dto.AverageDuration = 0;

            //
            // 3) StartPoint = ime prvog KeyPoint-a
            //
            var tourWithKeyPoints = _tourRepository.GetByIdWithKeyPoints(tour.Id);
            if (tourWithKeyPoints != null && tourWithKeyPoints.KeyPoints.Any())
            {
                var first = tourWithKeyPoints.KeyPoints.OrderBy(k => k.Id).First();
                dto.StartPoint = first.Name;
                dto.FirstKeyPoint = _mapper.Map<KeyPointDto>(first);
            }

            //
            // 4) AverageRating
            //
            var reviews = _reviewRepository.GetAllForTour(tour.Id);

            if (reviews.Any())
            {
                dto.AverageRating = reviews.Average(r => r.Rating);
                dto.Reviews = reviews.Select(r => _mapper.Map<TourReviewDto>(r)).ToList();
            }
            else
            {
                dto.AverageRating = 0;
                dto.Reviews = new List<TourReviewDto>();
            }
            foreach (var d in result)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"TOUR DEBUG -> {d.Name}: " +
                    $"DistanceInKm(DB)={_tourRepository.GetById(d.Id)?.DistanceInKm}, " +
                    $"Mapped Length={d.Length}, " +
                    $"AvgDur={d.AverageDuration}, " +
                    $"StartPoint={d.StartPoint}"
                );
            }

            result.Add(dto);
        }

        return result;
    }


    public List<TourPreviewDto> GetAvailableTours()
    {
        var tours = _tourRepository.GetPublished();
        return _mapper.Map<List<TourPreviewDto>>(tours);
    }

    public TourPreviewDto GetPreview(long tourId)
    {
        var tour = _tourRepository.GetById(tourId)
            ?? throw new NotFoundException("Tour not found.");

        return _mapper.Map<TourPreviewDto>(tour);
    }

    public TourDetailsDto GetDetails(long touristId, long tourId)
    {
        var tour = _tourRepository.GetTourWithKeyPoints(tourId)
            ?? throw new NotFoundException("Tour not found.");

        bool purchased = _access.HasUserPurchased(touristId, tourId);

        // MAP IRRELEVANT FIELDS
        var preview = _mapper.Map<TourDetailsDto>(tour);

        // LENGTH
        preview.Length = tour.DistanceInKm;

        // AVG DURATION
        preview.AverageDuration =
            tour.TourDurations != null && tour.TourDurations.Any()
                ? tour.TourDurations.Average(td => td.TimeInMinutes)
                : 0;

        // START POINT
        var first = tour.KeyPoints.OrderBy(k => k.Id).FirstOrDefault();
        preview.StartPoint = first?.Name ?? "";
        preview.FirstKeyPoint = first != null ? _mapper.Map<KeyPointDto>(first) : null;

        // 4) IMAGES — lista URL-ova iz KeyPoints
        preview.Images = tour.KeyPoints
            .Where(k => !string.IsNullOrWhiteSpace(k.ImageUrl))
            .Select(k => k.ImageUrl)
            .ToList();

        if (!purchased)
        {
            preview.KeyPoints = null;
            return preview;
        }

        preview.KeyPoints = tour.KeyPoints
            .Select(k => _mapper.Map<KeyPointPublicDto>(k))
            .ToList();

        return preview;
    }

}