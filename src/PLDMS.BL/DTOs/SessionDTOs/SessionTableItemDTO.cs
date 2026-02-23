using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record SessionTableItemDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CohortName { get; set; } = null!;
    public string ProgramName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalStudentCount { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public string RepositoryUrl { get; set; } = null!;
}