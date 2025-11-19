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
        CreateMap<Facility, FacilityDto>().ReverseMap();
    }
}
