using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Group : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    
    public Guid SessionId { get; set; }
    public Session Session { get; set; }
    
    public int TotalStudentCount { get; set; }
    
    public ICollection<Submission> Submissions { get; set; } = [];
    public ICollection<StudentGroup> Students { get; set; } = [];
}