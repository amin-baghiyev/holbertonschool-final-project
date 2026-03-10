using AutoMapper;
using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Profiles;

public class CohortProfile : Profile
{
    public CohortProfile()
    {
        CreateMap<Cohort, CohortFormDTO>().ReverseMap();

        CreateMap<Cohort, CohortOptionItemDTO>();
    }
}