using PLDMS.Core.Enums;

namespace PLDMS.BL.DTOs;

public record StudentSessionListItemDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SessionStatus SessionStatus { get; set; }
}
