namespace PLDMS.Core.Entities;

public class StudentCohort
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    
    public int CohortId { get; set; }
    public Cohort Cohort { get; set; }
}