using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Cohort : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalStudentCount { get; set; }
    public bool IsDeleted { get; set; }
    public int ProgramId { get; set; }
    public Program Program { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];
}