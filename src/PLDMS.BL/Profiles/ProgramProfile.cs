using AutoMapper;
using PLDMS.BL.DTOs.ProgramDTOs;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Profiles;

public class ProgramProfile : Profile
{
    public ProgramProfile()
    {
        CreateMap<Program, ProgramItemDTO>().ReverseMap();
        CreateMap<Program, ProgramFormDTO>().ReverseMap();
    }
}