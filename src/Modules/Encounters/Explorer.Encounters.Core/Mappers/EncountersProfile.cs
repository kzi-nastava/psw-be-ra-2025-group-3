using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;


namespace Explorer.Encounters.Core.Mappers
{
    public class EncountersProfile : Profile
    {
        public EncountersProfile() 
        {
            CreateMap<EncounterDto, Encounter>()
               .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new GeoPoint(src.Latitude, src.Longitude)))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<EncounterStatus>(src.Status)))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<EncounterType>(src.Type)))
               .ForMember(dest => dest.ActionDescription, opt => opt.MapFrom(src => src.ActionDescription));

            CreateMap<Encounter, EncounterDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Longitude))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.ActionDescription, opt => opt.MapFrom(src => src.ActionDescription));


        }
    }
}
