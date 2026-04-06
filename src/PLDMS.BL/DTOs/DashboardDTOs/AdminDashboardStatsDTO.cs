namespace PLDMS.BL.DTOs.DashboardDTOs;

public class SystemActivityDTO
{
    public string Type { get; set; } = null!;
    public string ActionTextHtml { get; set; } = null!;
    public string SubText { get; set; } = null!;
    public string IconClass { get; set; } = null!;
    public string BgClass { get; set; } = null!;
    public string TextColorClass { get; set; } = null!;
    public DateTime? Timestamp { get; set; }
}

public class AdminDashboardStatsDTO
{
    public int TotalUsers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalMentors { get; set; }
    public int ActiveCohorts { get; set; }
    public int ActivePrograms { get; set; }
    public int NewRegistrations { get; set; }
    public List<SystemActivityDTO> RecentActivities { get; set; } = new();
}