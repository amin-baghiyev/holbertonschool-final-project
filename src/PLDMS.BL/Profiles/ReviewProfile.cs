using AutoMapper;
using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Profiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, MentorReviewListItemDTO>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.SessionName, opt => opt.MapFrom(src => src.Group.Session.Name))
            .ForMember(dest => dest.ReviewerFullName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : string.Empty))
            .ForMember(dest => dest.ReviewerRole, opt => opt.MapFrom(src => src.Reviewer != null ? (UserRole?)src.Reviewer.Role : null))
            .ForMember(dest => dest.AssignedByFullName, opt => opt.MapFrom(src => src.AssignedBy != null ? src.AssignedBy.FullName : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ReviewStatus));
    }
}
