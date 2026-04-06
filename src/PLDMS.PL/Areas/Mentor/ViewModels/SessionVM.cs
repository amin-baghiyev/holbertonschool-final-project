using PLDMS.BL.DTOs;
using PLDMS.Core.Enums;

namespace PLDMS.PL.Areas.Mentor.ViewModels;

public record SessionVM
{
    public ICollection<SessionTableItemDTO> Sessions { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public SessionStatus? Status { get; set; }
    public int? CohortId { get; set; }
    public int? ProgramId { get; set; }
}