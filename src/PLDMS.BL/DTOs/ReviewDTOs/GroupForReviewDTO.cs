using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record GroupForReviewDTO
{
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string CohortName { get; init; } = string.Empty;
    public string ProgramName { get; init; } = string.Empty;
    public DateTime SessionStartDate { get; init; }
    public DateTime SessionEndDate { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public string? ReviewerFullName { get; init; }
}
