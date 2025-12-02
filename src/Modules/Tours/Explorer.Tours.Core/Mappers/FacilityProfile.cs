using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Mappers;

public class FacilityProfile : Profile
{
    public FacilityProfile()
    {

        CreateMap<Facility, FacilityDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => (int)src.Category));


        CreateMap<FacilityCreateDto, Facility>()
            .ConstructUsing(dto =>
                new Facility(
                    dto.Name,
                    dto.Latitude,
                    dto.Longitude,
                    (FacilityCategory)dto.Category
                )
            );


        CreateMap<FacilityUpdateDto, Facility>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id se ne menja
            .AfterMap((dto, entity) =>
            {
                entity.Update(
                    dto.Name,
                    dto.Latitude,
                    dto.Longitude,
                    (FacilityCategory)dto.Category
                );
            });
    }
}
