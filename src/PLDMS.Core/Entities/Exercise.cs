using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Exercise : BaseEntity<long>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExerciseDifficulty Difficulty { get; set; }
    public bool IsDeleted { get; set; }

    public int ProgramId { get; set; }
    public Program Program { get; set; }

    public ICollection<TestCase> TestCases { get; set; } = [];
    public ICollection<ExerciseLanguage> ExerciseLanguages { get; set; } = [];
}