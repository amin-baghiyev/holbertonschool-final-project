using PLDMS.BL.DTOs.MentorDTOs;

namespace PLDMS.PL.Areas.Admin.ViewModels;

public record MentorVM
{
     public ICollection<MentorTableItemDTO> Mentors { get; set; }
     public int TotalCount { get; set; }
     public int CurrentPage { get; set; }
     public int PageSize { get; set; }
     public string Search { get; set; }
}