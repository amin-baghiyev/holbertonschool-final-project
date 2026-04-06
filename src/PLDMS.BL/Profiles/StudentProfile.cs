using AutoMapper;
using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Profiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Session, StudentSessionListItemDTO>()
            .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => GetSessionStatus(src.StartDate, src.EndDate)))
            .ForMember(dest => dest.CohortName, opt => opt.MapFrom(src => src.Cohort != null ? src.Cohort.Name : string.Empty))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Cohort != null && src.Cohort.Program != null ? src.Cohort.Program.Name : string.Empty));

        CreateMap<Session, StudentSessionDetailDTO>()
            .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => GetSessionStatus(src.StartDate, src.EndDate)))
            .ForMember(dest => dest.CohortName, opt => opt.MapFrom(src => src.Cohort != null ? src.Cohort.Name : string.Empty))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Cohort != null && src.Cohort.Program != null ? src.Cohort.Program.Name : string.Empty))
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises.Select(e => e.Exercise)));

        CreateMap<Exercise, StudentSessionExerciseDTO>()
            .ForMember(dest => dest.Languages, opt => opt.MapFrom(src => src.ExerciseLanguages.Select(el => el.ProgrammingLanguage)));

        CreateMap<AppUser, TeammateDTO>();

        CreateMap<Review, StudentReviewListItemDTO>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.AssignedByName, opt => opt.MapFrom(src => src.AssignedBy.FullName));
    }

    private static SessionStatus GetSessionStatus(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow.AddHours(4);
        if (startDate > now) return SessionStatus.Upcoming;
        if (endDate < now) return SessionStatus.Finished;
        return SessionStatus.Active;
    }
}
