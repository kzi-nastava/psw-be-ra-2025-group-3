using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourAccessService _access;

    //private readonly ITourPurchaseTokenRepository _tokenRepository; //tour execution
    private readonly IInternalTokenService _tokenService; //tour execution

    private readonly IMapper _mapper;

    /*public TouristTourService(ITourRepository tourRepository, ITourReviewRepository reviewRepository, ITourAccessService access, ITourPurchaseTokenRepository tokenRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _reviewRepository = reviewRepository;
        _access = access;
        _tokenRepository = tokenRepository; // tour execution
        _mapper = mapper;
    }*/

    public TouristTourService(ITourRepository tourRepository, ITourReviewRepository reviewRepository, ITourAccessService access, IInternalTokenService tokenService, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _reviewRepository = reviewRepository;
        _access = access;

        //_tokenRepository = tokenRepository; // tour execution
        _tokenService = tokenService; // tour execution

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

    //tour execution
    public List<TourPreviewDto> GetMyPurchasedTours(long touristId)
    {
        //var tokens = _tokenRepository.GetByTouristId(touristId);
        //var tourIds = tokens.Select(t => t.TourId).Distinct().ToList();

        var tokens = _tokenService.GetTokens(touristId);
        var tourIds = tokens.Select(t => t.TourId).Distinct().ToList();


        //if (!tourIds.Any())
        //  return new List<TourPreviewDto>();

        if (!tourIds.Any())
            return new List<TourPreviewDto>();

        var result = new List<TourPreviewDto>();

        foreach (var tourId in tourIds)
        {
            var tour = _tourRepository.GetByIdWithKeyPoints(tourId);
            if (tour == null) continue;

            var dto = _mapper.Map<TourPreviewDto>(tour);

            dto.Length = tour.DistanceInKm;

            if (tour.TourDurations != null && tour.TourDurations.Any())
                dto.AverageDuration = tour.TourDurations.Average(td => td.TimeInMinutes);
            else
                dto.AverageDuration = 0;

            if (tour.KeyPoints != null && tour.KeyPoints.Any())
            {
                var first = tour.KeyPoints.OrderBy(k => k.Id).First();
                dto.StartPoint = first.Name;
                dto.FirstKeyPoint = _mapper.Map<KeyPointDto>(first);
            }

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

            result.Add(dto);
        }

        return result;
    }

    public List<TourPreviewDto> SearchAndFilterTours(TourFilterDto filters)
    {
        // Pozivamo repository metodu sa filterima
        var tours = _tourRepository.SearchAndFilter(
            filters.Name, 
            filters.Tags, 
            filters.Difficulties, 
            filters.MinPrice, 
            filters.MaxPrice
        );

        var result = new List<TourPreviewDto>();

        foreach (var tour in tours)
        {
            var dto = _mapper.Map<TourPreviewDto>(tour);

            // LENGTH
            dto.Length = tour.DistanceInKm;

            // AVERAGE DURATION
            if (tour.TourDurations != null && tour.TourDurations.Any())
                dto.AverageDuration = tour.TourDurations.Average(td => td.TimeInMinutes);
            else
                dto.AverageDuration = 0;

            // START POINT
            var tourWithKeyPoints = _tourRepository.GetByIdWithKeyPoints(tour.Id);
            if (tourWithKeyPoints != null && tourWithKeyPoints.KeyPoints.Any())
            {
                var first = tourWithKeyPoints.KeyPoints.OrderBy(k => k.Id).First();
                dto.StartPoint = first.Name;
                dto.FirstKeyPoint = _mapper.Map<KeyPointDto>(first);
            }

            // AVERAGE RATING
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

            result.Add(dto);
        }

        // MINIMUM RATING (filtrira NULL rating-e)
        if (filters.MinRating.HasValue)
        {
            // Eliminiše ture sa rating = 0 (bez recenzija) kada je filter aktivan
            result = result.Where(t => t.AverageRating >= filters.MinRating.Value).ToList();
        }

        return result;
    }
}