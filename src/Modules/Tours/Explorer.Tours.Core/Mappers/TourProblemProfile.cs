using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mapper;

public class TourProblemProfile : Profile
{
    public TourProblemProfile()
    {
        CreateMap<TourProblem, TourProblemDto>().ReverseMap();
        CreateMap<TourProblemCreateDto, TourProblem>();
        CreateMap<TourProblemUpdateDto, TourProblem>();
    }
}