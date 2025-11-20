using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMeetupRepository
    {
        List<Meetup> GetAll();
        Meetup? GetById(long id);
        Meetup Create(Meetup meetup);
        Meetup Update(Meetup meetup);
        void Delete(Meetup meetup);
    }
}
