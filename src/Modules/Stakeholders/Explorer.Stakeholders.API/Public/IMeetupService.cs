using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMeetupService
    {
        List<MeetupDto> GetAll();
        MeetupDto GetById(long id);
        MeetupDto Create(MeetupCreateDto meetup, long creatorId);
        MeetupDto Update(long id, MeetupUpdateDto meetup, long creatorId);
        void Delete(long id, long creatorId);
        List<MeetupDto> GetByTourId(long tourId);
    }
}
