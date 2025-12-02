// Explorer.Tours.Infrastructure/Database/Repositories/TourReviewDbRepository.cs
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourReviewDbRepository : ITourReviewRepository
{
    private readonly ToursContext _context;

    public TourReviewDbRepository(ToursContext context)
    {
        _context = context;
    }

    public TourReview Create(TourReview review)
    {
        _context.TourReviews.Add(review);
        _context.SaveChanges();
        return review;
    }

    public TourReview Update(TourReview review)
    {
        _context.TourReviews.Update(review);
        _context.SaveChanges();
        return review;
    }

    public TourReview? GetById(long id)
    {
        return _context.TourReviews.FirstOrDefault(r => r.Id == id);
    }

    public TourReview? GetByTouristAndTour(long touristId, long tourId)
    {
        return _context.TourReviews
            .FirstOrDefault(r => r.TouristId == touristId && r.TourId == tourId);
    }

    public List<TourReview> GetAllForTour(long tourId)
    {
        return _context.TourReviews
            .Where(r => r.TourId == tourId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public bool HasReview(long touristId, long tourId)
    {
        return _context.TourReviews
            .Any(r => r.TouristId == touristId && r.TourId == tourId);
    }
}