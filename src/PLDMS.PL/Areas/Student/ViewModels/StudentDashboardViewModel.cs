using PLDMS.BL.DTOs;

namespace PLDMS.PL.Areas.Student.ViewModels;

public class StudentDashboardViewModel
{
    public ICollection<StudentSessionListItemDTO> ActiveSessions { get; set; } = new List<StudentSessionListItemDTO>();
    public ICollection<StudentSessionListItemDTO> UpcomingSessions { get; set; } = new List<StudentSessionListItemDTO>();
    public ICollection<StudentReviewListItemDTO> RecentReviews { get; set; } = new List<StudentReviewListItemDTO>();
}
