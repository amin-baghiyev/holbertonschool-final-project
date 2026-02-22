using PLDMS.Core.Entities.Base;
using PLDMS.Core.Enums;

namespace PLDMS.Core.Entities;

public class Review : BaseEntity<int>
{
    public Guid ReviewerId { get; set; }
    public AppUser Reviewer { get; set; }
    
    public Guid AssignedById { get; set; }
    public AppUser AssignedBy { get; set; }
    
    public int GroupId { get; set; }
    public Group Group { get; set; }
    
    public int Score { get; set; }
    public string Note { get; set; }
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}