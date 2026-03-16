namespace PLDMS.BL.DTOs;

public record AddStudentsRequest
{
    public int CohortId { get; set; }
    public IEnumerable<Guid> StudentIds { get; set; }
}