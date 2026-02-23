using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Task : BaseEntity<long>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TaskDifficulty Difficulty { get; set; }
    public bool IsDeleted { get; set; }

    public int ProgramId { get; set; }
    public Program Program { get; set; }

    public ICollection<TestCase> TestCases { get; set; } = [];
    public ICollection<TaskLanguage> TaskLanguages { get; set; } = [];
}