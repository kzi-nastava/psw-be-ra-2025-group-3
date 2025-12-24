using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourProblemService
{
    TourProblemDto Create(TourProblemCreateDto problemDto, long touristId);
    TourProblemDto Update(TourProblemUpdateDto problemDto, long touristId);
    void Delete(long id, long touristId);
    TourProblemDto GetById(long id, long touristId);
    List<TourProblemDto> GetByTouristId(long touristId);

    //Podtask 1
    TourProblemDto MarkAsResolved(long problemId, string touristComment, long touristId);
    TourProblemDto MarkAsUnresolved(long problemId, string touristComment, long touristId);
    List<TourProblemDto> GetByAuthorId(long authorId);
    TourProblemDto AddMessage(long problemId, long authorId, string content, int authorType);

    List<TourProblemDto> GetByTourId(long tourId);
}