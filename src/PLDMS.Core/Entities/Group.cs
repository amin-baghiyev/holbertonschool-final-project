using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Group : BaseEntity<int>
{
    public string Name { get; set; }
    
    public int SessionId { get; set; }
    public Session Session { get; set; }
    
    public int SessionCount { get; set; }
    
    public ICollection<Review> Reviews { get; set; }
    
    public ICollection<Submission> Submissions { get; set; }
}