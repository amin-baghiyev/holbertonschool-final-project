using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using AutoMapper;

namespace PLDMS.BL.Profiles;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<SessionFormDTO, Session>()
            .ForMember(dest => dest.Exercises, opt => opt.Ignore());

        CreateMap<Session, SessionFormDTO>()
            .ForMember(dest => dest.ExercisesIds, opt => opt.Ignore());

        CreateMap<Session, SessionTableItemDTO>()
            .ForMember(dest => dest.CohortName, opt => opt.MapFrom(src => src.Cohort != null ? src.Cohort.Name : string.Empty))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Cohort != null && src.Cohort.Program != null ? src.Cohort.Program.Name : string.Empty))
            .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => GetSessionStatus(src.StartDate, src.EndDate)));

        CreateMap<Session, SessionDetailDTO>()
            .ForMember(dest => dest.CohortName, opt => opt.MapFrom(src => src.Cohort != null ? src.Cohort.Name : string.Empty))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Cohort != null && src.Cohort.Program != null ? src.Cohort.Program.Name : string.Empty))
            .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => GetSessionStatus(src.StartDate, src.EndDate)))
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises.Select(se => se.Exercise)));

        CreateMap<Group, GroupListItemDTO>();
        CreateMap<StudentGroup, StudentListItemDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Student.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Student.FullName));

        CreateMap<Exercise, ExerciseListItemDTO>()
            .ForMember(dest => dest.Languages, opt => opt.MapFrom(src => src.ExerciseLanguages.Select(el => el.ProgrammingLanguage)));
    }

    private static SessionStatus GetSessionStatus(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow.AddHours(4);
        if (startDate > now) return SessionStatus.Upcoming;
        if (endDate < now) return SessionStatus.Finished;
        return SessionStatus.Active;
    }
}