using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Mappers;

public class EncounterActivationProfile : Profile
{
    public EncounterActivationProfile()
    {
        CreateMap<EncounterActivation, EncounterActivationDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<EncounterActivationDto, EncounterActivation>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<EncounterActivationStatus>(src.Status)));
    }
}