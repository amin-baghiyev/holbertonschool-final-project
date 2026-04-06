using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record StudentSessionDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SessionStatus SessionStatus { get; set; }
    
    public string CohortName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    
    public Guid AssignedGroupId { get; set; }
    public string AssignedGroupName { get; set; } = null!;
    
    public IEnumerable<StudentSessionExerciseDTO> Exercises { get; set; } = [];
    public ICollection<TeammateDTO> Teammates { get; set; } = [];
}

public record StudentSessionExerciseDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public ICollection<ProgrammingLanguage> Languages { get; set; } = [];
    public int? PassCount { get; set; }
    public int? TotalTests { get; set; }
    public bool IsSolved { get; set; }
}

public record TeammateDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
}
