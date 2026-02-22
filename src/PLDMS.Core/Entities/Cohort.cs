using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Cohort : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int StudentCount { get; set; }
    public bool IsDeleted { get; set; }
}