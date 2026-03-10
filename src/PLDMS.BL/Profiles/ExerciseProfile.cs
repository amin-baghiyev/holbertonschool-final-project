using AutoMapper;
using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Profiles;

public class ExerciseProfile : Profile
{
    public ExerciseProfile()
    {
        CreateMap<TestCase, ExerciseTestCasesDTO>().ReverseMap();

        CreateMap<Exercise, ExerciseTableItemDTO>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.Name))
            .ForMember(dest => dest.Languages, opt => opt.MapFrom(src =>
                src.ExerciseLanguages.Select(el => el.ProgrammingLanguage)));

        CreateMap<Exercise, ExerciseDetailDTO>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.Name))
            .ForMember(dest => dest.Languages, opt => opt.MapFrom(src =>
                src.ExerciseLanguages.Select(el => el.ProgrammingLanguage)));

        CreateMap<Exercise, ExerciseFormDTO>()
            .ForMember(dest => dest.Languages, opt => opt.MapFrom(src =>
                src.ExerciseLanguages.Select(el => el.ProgrammingLanguage)));

        CreateMap<Exercise, ExerciseAsOptionDTO>();

        CreateMap<ExerciseFormDTO, Exercise>()
            .ForMember(dest => dest.ExerciseLanguages, opt => opt.MapFrom(src =>
                src.Languages.Select(lang => new ExerciseLanguage { ProgrammingLanguage = lang })))
            .ForMember(dest => dest.TestCases, opt => opt.MapFrom(src => src.TestCases));
    }
}