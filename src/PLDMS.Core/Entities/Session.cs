using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Session : BaseEntity<int>
{
    public string Name { get; set; }
    
    public int CohortId { get; set; }
    public Cohort Cohort { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int StudentCountPerGroup { get; set; }
    public int StudentCount { get; set; }
    public ReviewStatus ReviewStatus { get; set; }
    
    public ICollection<Group> Groups { get; set; }
}