using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Task : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public int ProgramId { get; set; }
    public Program Program { get; set; }
    
    public TaskDifficulty Difficulty { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Submission> Submissions { get; set; }
    
    public ICollection<TestCase> TestCases { get; set; }

    public ICollection<TaskLanguage> TaskLanguages { get; set; }
}