using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourReviewRepository
{
    TourReview Create(TourReview review);
    TourReview Update(TourReview review);
    TourReview? GetById(long id);
    TourReview? GetByTouristAndTour(long touristId, long tourId);
    List<TourReview> GetAllForTour(long tourId);
    bool HasReview(long touristId, long tourId);
    TourReview? GetByIdWithImages(long id);
}