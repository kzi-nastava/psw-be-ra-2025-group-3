using AutoMapper;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<AccountDto, Account>().ReverseMap(); //anja dodala
        CreateMap<AccountCreateDto, Account>(); //anja dodala
        
    }
}