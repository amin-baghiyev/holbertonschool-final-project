using PLDMS.Core.Entities.Base;

namespace PLDMS.Core.Entities;

public class TestCase : BaseEntity<long>
{
    public string Input { get; set; } = null!;
    public string Output { get; set; } = null!;
    public bool IsDeleted { get; set; }

    public long ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
}