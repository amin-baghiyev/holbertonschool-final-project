using PLDMS.BL.DTOs.ProgramDTOs;

namespace PLDMS.BL.DTOs.CohortDTOs;

public record CohortTableItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ProgramSelectItemDTO Program { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int StudentCount { get; set; }
    public bool IsDeleted { get; set; }
}