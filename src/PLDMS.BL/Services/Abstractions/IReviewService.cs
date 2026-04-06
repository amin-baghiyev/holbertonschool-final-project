using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Services.Abstractions;

public interface IReviewService
{
    Task<(ICollection<MentorReviewListItemDTO> Items, int TotalCount)> GetMentorReviewsAsync(string? q = null, ReviewStatus? status = null, int page = 0, int count = 25);
    Task<(ICollection<GroupForReviewDTO> Items, int TotalCount)> GetGroupsNeedingReviewAsync(string? q = null, ReviewStatus? status = null, int? cohortId = null, int? programId = null, int page = 0, int count = 25);
    Task AssignReviewAsync(Guid mentorId, MentorAssignReviewDTO dto);
    Task GiveReviewAsync(Guid mentorId, MentorReviewCreateDTO dto);
    Task UpdateMentorReviewStatusAsync(Guid reviewId, ReviewStatus status);
    Task<ReviewDetailDTO> GetReviewDetailAsync(Guid reviewId);
    Task<ReviewDetailDTO> GetReviewDetailForGroupAsync(Guid groupId);
    Task<ReviewSummaryDTO> GetReviewSummaryAsync();
}
