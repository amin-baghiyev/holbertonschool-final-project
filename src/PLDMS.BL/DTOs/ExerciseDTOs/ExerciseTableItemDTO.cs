using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ExerciseTableItemDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public string ProgramName { get; set; } = null!;
    public ICollection<ProgrammingLanguage> Languages { get; set; } = [];
    public bool IsDeleted { get; set; }
}