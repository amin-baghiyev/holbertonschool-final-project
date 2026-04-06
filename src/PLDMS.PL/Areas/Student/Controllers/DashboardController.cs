using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;
using PLDMS.PL.Areas.Student.ViewModels;
using System.Security.Claims;

namespace PLDMS.PL.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public class DashboardController : Controller
{
    private readonly IStudentService _studentService;

    public DashboardController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Login", "Auth", new { area = "Auth" });

        var activeSessionsResult = await _studentService.GetSessionsAsync(
            studentId, status: SessionStatus.Active, page: 0, count: 5);
            
        var upcomingSessionsResult = await _studentService.GetSessionsAsync(
            studentId, status: SessionStatus.Upcoming, page: 0, count: 5);

        var recentReviewsResult = await _studentService.GetReviewsAsync(
            studentId, page: 0, count: 5);

        var vm = new StudentDashboardViewModel
        {
            ActiveSessions = activeSessionsResult.Items,
            UpcomingSessions = upcomingSessionsResult.Items,
            RecentReviews = recentReviewsResult.Items
        };

        return View(vm);
    }
}