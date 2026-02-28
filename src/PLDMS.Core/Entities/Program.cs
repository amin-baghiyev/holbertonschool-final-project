using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Program : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
    public bool IsDeleted { get; set; }
}