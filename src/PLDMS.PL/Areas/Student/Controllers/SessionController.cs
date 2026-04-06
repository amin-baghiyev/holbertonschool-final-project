using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public async Task<IActionResult> Index(string? q, int? cohortId, int? programId, SessionStatus? status, int page = 1, int pageSize = 10)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        pageSize = pageSize switch
        {
            5 => 5,
            10 => 10,
            15 => 15,
            20 => 20,
            25 => 25,
            _ => 10
        };

        var (items, totalCount) = await _studentService.GetSessionsAsync(studentId, q, null, null, status, page - 1, pageSize, cohortId, programId);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages  = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.TotalCount  = totalCount;
        ViewBag.PageSize    = pageSize;

        var (cohorts, programs) = await _studentService.MySessionFilterOptionsAsync(studentId);
        ViewBag.Programs = new SelectList(programs, "Id", "Name", programId);
        ViewBag.Cohorts  = new SelectList(cohorts,  "Id", "Name", cohortId);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            var dto = await _studentService.GetSessionDetailAsync(id, studentId);

            if (dto.SessionStatus == SessionStatus.Upcoming)
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
