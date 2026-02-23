using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class TestCase : BaseEntity<long>
{
    public string Input { get; set; } = null!;
    public string Output { get; set; } = null!;
    public bool IsDeleted { get; set; }

    public long TaskId { get; set; }
    public Task Task { get; set; }
}