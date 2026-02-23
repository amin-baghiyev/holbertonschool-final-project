namespace PLDMS.Core.Entities;

public class StudentCohort
{
    public Guid StudentId { get; set; }
    public AppUser Student { get; set; }
    public int CohortId { get; set; }
    public Cohort Cohort { get; set; }
    public bool IsDeleted { get; set; }
}