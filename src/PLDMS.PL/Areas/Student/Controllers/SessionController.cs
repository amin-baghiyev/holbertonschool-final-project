using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;
using System.Security.Claims;

namespace PLDMS.PL.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public class SessionController : Controller
{
    private readonly IStudentService _studentService;

    public SessionController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, DateTime? startDate, DateTime? endDate, SessionStatus? status, int page = 1)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        int pageSize = 10;
        var (items, totalCount) = await _studentService.GetSessionsAsync(studentId, q, startDate, endDate, status, page - 1, pageSize);
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.TotalCount = totalCount;

        return View(items);
    }
}
