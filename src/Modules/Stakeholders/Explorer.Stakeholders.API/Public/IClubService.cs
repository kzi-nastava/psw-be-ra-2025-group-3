using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        PagedResult<ClubDto> GetPaged(int page, int pageSize);
        ClubDto Get(long id);
        ClubDto Create(ClubCreateDto club, long userId);
        ClubDto Update(long id, ClubUpdateDto club, long userId);
        void Delete(long id, long userId);
        PagedResult<ClubDto> GetUserClubs(long userId, int page, int pageSize);
    }
}
