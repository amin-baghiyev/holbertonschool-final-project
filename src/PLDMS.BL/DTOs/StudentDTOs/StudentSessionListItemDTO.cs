using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record StudentSessionListItemDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int GroupStudentCount { get; set; }
    public string CohortName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int ExercisesCount { get; set; }
    public int SolvedExercisesCount { get; set; }
}
