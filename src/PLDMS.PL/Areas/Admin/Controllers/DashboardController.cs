using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.Services.Abstractions;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _dashboardService.AdminDashboardStatsAsync();
        return View(stats);
    }
}