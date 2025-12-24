using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourProblemRepository
{
    TourProblem Create(TourProblem problem);
    TourProblem Update(TourProblem problem);
    void Delete(long id);
    TourProblem GetById(long id);
    List<TourProblem> GetByTouristId(long touristId);

    //Podtask 1
    List<TourProblem> GetByAuthorId(long authorId);

    // Podtask 4 - za admina:
    List<TourProblem> GetAll(); 
    List<TourProblem> GetOverdue(int daysThreshold);

    List<TourProblem> GetByTourId(long tourId);
}