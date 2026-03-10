using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ExerciseDetailDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public bool IsDeleted { get; set; }
    public string ProgramName { get; set; } = null!;
    public IEnumerable<ExerciseTestCasesDTO> TestCases { get; set; } = [];
    public IEnumerable<ProgrammingLanguage> Languages { get; set; } = [];
}