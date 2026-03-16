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
            .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => GetSessionStatus(src.StartDate, src.EndDate)));

        CreateMap<Review, StudentReviewListItemDTO>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.AssignedByName, opt => opt.MapFrom(src => src.AssignedBy.FullName));
    }

    private static SessionStatus GetSessionStatus(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow;
        if (startDate > now) return SessionStatus.Upcoming;
        if (endDate < now) return SessionStatus.Finished;
        return SessionStatus.Active;
    }
}
