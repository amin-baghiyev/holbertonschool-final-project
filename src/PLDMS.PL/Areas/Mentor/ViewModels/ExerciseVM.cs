using PLDMS.BL.DTOs;

namespace PLDMS.PL.Areas.Mentor.ViewModels;

public record ExerciseVM
{
    public ICollection<ExerciseTableItemDTO>? Exercises { get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}