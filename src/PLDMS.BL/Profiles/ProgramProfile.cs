using AutoMapper;
using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Profiles;

public class ProgramProfile : Profile
{
    public ProgramProfile()
    {
        CreateMap<Program, ProgramItemDTO>().ReverseMap();
        CreateMap<Program, ProgramFormDTO>().ReverseMap();
        CreateMap<Program, ProgramOptionItemDTO>();
    }
}