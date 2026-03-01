using PLDMS.BL.DTOs.CohortDTOs;

namespace PLDMS.PL.Areas.Admin.ViewModels;

public record CohortVM
{
    public ICollection<CohortTableItemDTO>? Cohorts{ get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string? Search{ get; set; }
}