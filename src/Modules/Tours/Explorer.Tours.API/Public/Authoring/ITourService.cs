using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface ITourService
{
    TourDto Create(TourCreateDto tourDto, long authorId);
    TourDto Update(TourUpdateDto tourDto, long authorId);
    void Delete(long id, long authorId);
    TourDto GetById(long id);
    List<TourDto> GetByAuthorId(long authorId);
    TourDto Publish(long id, long authorId);
    TourDto AddEquipment(long tourId, long equipmentId, long authorId);
    TourDto RemoveEquipment(long tourId, long equipmentId, long authorId);
}