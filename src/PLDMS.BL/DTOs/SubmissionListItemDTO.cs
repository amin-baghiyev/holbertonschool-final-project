using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record SubmissionListItemDTO
{
    public Guid Id { get; set; }
    public string CommitHash { get; set; } = null!;
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
    public int PassCount { get; set; }
    public int TotalTests { get; set; }
    public bool Success => PassCount == TotalTests;
    public DateTime CreatedAt { get; set; }
}
