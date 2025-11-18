using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<AccountDto, Account>().ReverseMap(); //anja dodala
        CreateMap<AccountCreateDto, Account>(); //anja dodala

                                                
        CreateMap<Account, AccountDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Person, PersonDto>().ReverseMap();

        // --- Dodao petar s. 
        CreateMap<Club, ClubDto>()
            .ForMember(dest => dest.FeaturedImage, opt => opt.MapFrom(src => src.FeaturedImage))
            .ForMember(dest => dest.GalleryImages, opt => opt.MapFrom(src => src.Images.Where(img => img.Id != src.FeaturedImageId).ToList()));

        CreateMap<ClubImage, ClubImageDto>();
        // ----
    }
}