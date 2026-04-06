using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record MentorReviewListItemDTO
{
    public Guid Id { get; init; }
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = null!;
    public string SessionName { get; init; } = null!;
    public Guid? ReviewerId { get; init; }
    public string ReviewerFullName { get; init; } = string.Empty;
    public UserRole? ReviewerRole { get; init; }
    public Guid? AssignedById { get; init; }
    public string AssignedByFullName { get; init; } = string.Empty;
    public int Score { get; init; }
    public ReviewStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
