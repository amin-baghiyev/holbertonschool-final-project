using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Session : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string RepositoryUrl { get; set; } = null!;

    public int CohortId { get; set; }
    public Cohort Cohort { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int StudentCountPerGroup { get; set; }
    public int TotalStudentCount { get; set; }

    public ICollection<Group> Groups { get; set; } = [];
    public ICollection<SessionExercise> Exercises { get; set; } = [];
}