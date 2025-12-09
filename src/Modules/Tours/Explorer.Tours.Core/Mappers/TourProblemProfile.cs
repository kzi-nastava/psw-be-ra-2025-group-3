using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mapper;

public class TourProblemProfile : Profile
{
    public TourProblemProfile()
    {
        CreateMap<TourProblem, TourProblemDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));

        CreateMap<TourProblemDto, TourProblem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourProblemStatus)src.Status));

        CreateMap<TourProblemCreateDto, TourProblem>();
        CreateMap<TourProblemUpdateDto, TourProblem>();

        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.AuthorType, opt => opt.MapFrom(src => (int)src.AuthorType));

        CreateMap<TourProblem, AdminTourProblemDto>()
            .ForMember(dest => dest.TourName, opt => opt.Ignore()) 
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore())
            .ForMember(dest => dest.DaysOpen, opt => opt.Ignore());
    }
}