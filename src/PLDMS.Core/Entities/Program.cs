using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Program : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsDeleted { get; set; }
    
    public ICollection<Task> Tasks { get; set; }
}