using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ReviewDetailDTO
{
    public Guid ReviewId { get; init; }
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string RepositoryUrl { get; init; } = string.Empty;
    public Guid? ReviewerId { get; init; }
    public int Score { get; init; }
    public string Note { get; init; } = string.Empty;
    public ReviewStatus Status { get; init; }

    public ICollection<SubmissionForReviewDTO> Submissions { get; init; } = new List<SubmissionForReviewDTO>();
}

public record SubmissionForReviewDTO
{
    public long ExerciseId { get; init; }
    public string ExerciseName { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string CommitHash { get; init; } = null!;
    public string BranchName { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public string SourceCode { get; init; } = null!;
    public ProgrammingLanguage Language { get; init; }
    public bool[] Tests { get; init; } = [];
}
