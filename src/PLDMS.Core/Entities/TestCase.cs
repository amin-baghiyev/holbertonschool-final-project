using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class TestCase : BaseEntity<int>
{
    public string Input { get; set; }
    public string Output { get; set; }
 
    public int TaskId { get; set; }
    public Task Task { get; set; }
    
    public bool IsDeleted { get; set; }
}