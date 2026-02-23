using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class TaskLanguage
{
    public long TaskId { get; set; }
    public Task Task { get; set; }
    
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
}