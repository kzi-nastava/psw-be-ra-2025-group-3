using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

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
