using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record ExerciseListItemDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public ICollection<ProgrammingLanguage> Languages { get; set; } = [];
}