using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class Student : BaseEntity<Guid>
{
    public string GitHub { get; set; }
}