using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using System.Linq;

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
        CreateMap<TourDurationDto, TourDuration>().ReverseMap();
        CreateMap<TourExecutionDto, TourExecution>().ReverseMap();

        CreateMap<KeyPointCompletion, KeyPointCompletionDto>();
        CreateMap<TourReview, TourReviewDto>()
            .ForMember(dest => dest.TouristName, opt => opt.Ignore());  
        CreateMap<TourReviewDto, TourReview>();

        CreateMap<ReviewImage, ReviewImageDto>();
        CreateMap<KeyPointPublicDto, KeyPoint>().ReverseMap();

       
        CreateMap<Tour, TourPreviewDto>()
            // 1. Difficulty kao int 
            .ForMember(dest => dest.Difficulty, 
                opt => opt.MapFrom(src => (int)src.Difficulty))
            
            // 2. LENGTH = DistanceInKm
            .ForMember(dest => dest.Length,
                opt => opt.MapFrom(src => src.DistanceInKm))

            // 3. AVERAGE DURATION iz JSON liste TourDurations
            .ForMember(dest => dest.AverageDuration,
                opt => opt.MapFrom(src =>
                    src.TourDurations != null && src.TourDurations.Any()
                        ? src.TourDurations.Average(td => td.TimeInMinutes)
                        : 0))

            // 4. START POINT – prvi key point po ID
            .ForMember(dest => dest.StartPoint,
                opt => opt.MapFrom(src =>
                    src.KeyPoints != null && src.KeyPoints.Any()
                        ? src.KeyPoints.OrderBy(kp => kp.Id).First().Name
                        : string.Empty))

            // 5. Properti-je koje popunjavamo rucno u servisu
            .ForMember(dest => dest.FirstKeyPoint, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        
        CreateMap<TourDetailsDto, Tour>().ReverseMap();
    }
}