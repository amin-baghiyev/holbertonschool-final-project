using PLDMS.BL.DTOs;

namespace PLDMS.PL.Areas.Admin.ViewModels;

public record StudentVM
{
    public ICollection<StudentTableItemDTO> Students { get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; }
}