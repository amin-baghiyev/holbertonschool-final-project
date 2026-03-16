using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;
using System.Security.Claims;

namespace PLDMS.PL.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public class ReviewController : Controller
{
    private readonly IStudentService _studentService;

    public ReviewController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index(ReviewStatus? status, int page = 1)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        int pageSize = 10;
        var (items, totalCount) = await _studentService.GetReviewsAsync(studentId, status, page - 1, pageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.TotalCount = totalCount;

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        var dto = await _studentService.GetReviewForEditAsync(id, studentId);
        if (dto == null)
            return NotFound("Review not found or not assigned to you.");

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid id, StudentReviewCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            await _studentService.SubmitReviewAsync(id, studentId, dto);
            TempData["SuccessMessage"] = "Review submitted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}
