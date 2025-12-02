using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();

        CreateMap<TourCreateDto, Tour>();
        CreateMap<TourUpdateDto, Tour>();

        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<TourPurchaseTokenDto, TourPurchaseToken>().ReverseMap();

       
        CreateMap<PositionDto, Position>().ReverseMap();


        CreateMap<AwardEvent, AwardEventDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<AwardEventCreateDto, AwardEvent>();
        CreateMap<AwardEventUpdateDto, AwardEvent>();
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        CreateMap<PagedResult<AwardEvent>, PagedResult<AwardEventDto>>();

        CreateMap<TourProblem, TourProblemDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<TourProblemDto, TourProblem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TourProblemStatus)src.Status));

        CreateMap<TourProblemCreateDto, TourProblem>();
        CreateMap<TourProblemUpdateDto, TourProblem>();

        CreateMap<OrderItem, ShoppingCartItemDto>();
        CreateMap<ShoppingCart, ShoppingCartDto>();
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();

        CreateMap<TourExecutionDto, TourExecution>().ReverseMap();
        CreateMap<TourReview, TourReviewDto>()
            .ForMember(dest => dest.TouristName, opt => opt.Ignore());  
        CreateMap<TourReviewDto, TourReview>();


    }
}