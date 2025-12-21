using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

public class DiaryProfile : Profile
{
    public DiaryProfile()
    {
        CreateMap<Diary, DiaryDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => (int)s.Status));
    }
}