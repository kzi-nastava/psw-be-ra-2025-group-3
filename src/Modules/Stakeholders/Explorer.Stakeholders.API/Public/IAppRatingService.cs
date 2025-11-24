using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IAppRatingService
    {
        AppRatingResponseDto CreateRating(long userId, AppRatingRequestDto rating);
        AppRatingResponseDto UpdateRating(long userId, AppRatingRequestDto rating);
        void DeleteRating(long userId);
        AppRatingResponseDto? GetMyRating(long userId);
        PagedResult<AppRatingResponseDto> GetPaged(int page, int pageSize);
    }
}

