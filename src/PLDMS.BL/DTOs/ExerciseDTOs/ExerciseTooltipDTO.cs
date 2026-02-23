using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ExerciseTooltipDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public ICollection<ProgrammingLanguage> Languages { get; set; } = [];
}