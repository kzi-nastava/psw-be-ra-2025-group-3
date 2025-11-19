using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IFacilityRepository
{
    Facility Create(Facility facility);
    Facility Update(Facility facility);
    void Delete(int id);
    Facility Get(int id);
    List<Facility> GetAll();
}

