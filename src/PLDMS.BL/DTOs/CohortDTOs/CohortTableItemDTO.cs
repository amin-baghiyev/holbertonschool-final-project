using PLDMS.BL.DTOs;

namespace PLDMS.BL.DTOs;

public record CohortTableItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ProgramOptionItemDTO Program { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int StudentCount { get; set; }
    public bool IsDeleted { get; set; }
}