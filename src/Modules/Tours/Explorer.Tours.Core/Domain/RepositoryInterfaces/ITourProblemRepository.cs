using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourProblemRepository
{
    TourProblem Create(TourProblem problem);
    TourProblem Update(TourProblem problem);
    void Delete(long id);
    TourProblem GetById(long id);
    List<TourProblem> GetByTouristId(long touristId);
}