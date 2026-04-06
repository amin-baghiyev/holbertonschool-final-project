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
    private readonly IReviewService _reviewService;

    public ReviewController(IStudentService studentService, IReviewService reviewService)
    {
        _studentService = studentService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index(ReviewStatus? status, int page = 1, int pageSize = 10)
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

        var (items, totalCount) = await _studentService.GetReviewsAsync(studentId, status, page - 1, pageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.TotalCount = totalCount;
        ViewBag.PageSize = pageSize;

        return View(items);
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
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                return Ok(new { message = "Review submitted successfully!" });
            TempData["SuccessMessage"] = "Review submitted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                return BadRequest(new { message = ex.Message });
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            var reviewDto = await _reviewService.GetReviewDetailAsync(id);
            if (reviewDto.ReviewerId != studentId)
                return Forbid();
            return View("~/Areas/Mentor/Views/Review/Detail.cshtml", reviewDto);
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
