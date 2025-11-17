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

    }
}