using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Submission : BaseEntity<Guid>
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    
    public long TaskId { get; set; }
    public Task Task { get; set; }

    public string CommitHash { get; set; } = null!;
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
    public bool[] Tests { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}