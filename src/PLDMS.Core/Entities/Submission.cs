using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Submission : BaseEntity<int>
{
    public int GroupId { get; set; }
    public Group Group { get; set; }
    
    public int TaskId { get; set; }
    public Task Task { get; set; }
    
    public string RepositoryUrl { get; set; }
    public string CommitHash { get; set; }
    public string BranchName { get; set; }
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
    public int CorrectTestCount { get; set; }
    public int TotalTestCount { get; set; }
    public DateTime CreatedAt { get; set; }
}