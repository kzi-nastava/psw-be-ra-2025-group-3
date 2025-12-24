using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases;

public class InternalTourService : IInternalTourService
{
    private readonly ITourRepository _tourRepository;

    public InternalTourService(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public List<TourForRecommendationDto> GetPublishedToursForRecommendation()
    {
        var tours = _tourRepository.GetPublishedTours();

        return tours.Select(t => new TourForRecommendationDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Difficulty = (int)t.Difficulty,
            Price = t.Price,
            DistanceInKm = t.DistanceInKm,
            Tags = t.Tags ?? new List<string>(),
            TransportationTypes = t.TourDurations?
                .Select(td => (int)td.TransportType)
                .Distinct()
                .ToList() ?? new List<int>()
        }).ToList();
    }
}