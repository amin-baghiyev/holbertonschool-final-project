using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;
using System.Security.Claims;

namespace PLDMS.PL.Areas.Mentor.Controllers;

[Area("Mentor")]
[Authorize(Roles = nameof(UserRole.Mentor))]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICohortService _cohortService;

    public ReviewController(IReviewService reviewService, UserManager<AppUser> userManager, ICohortService cohortService)
    {
        _reviewService = reviewService;
        _userManager = userManager;
        _cohortService = cohortService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, ReviewStatus? status, int page = 0, int pageSize = 10)
    {
        var (reviews, count) = await _reviewService.GetMentorReviewsAsync(q, status, page, pageSize);
        var summary = await _reviewService.GetReviewSummaryAsync();

        ViewBag.Search = q;
        ViewBag.Status = status;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = count;
        ViewBag.Summary = summary;

        return View(reviews);
    }

    [HttpGet]
    public async Task<IActionResult> AssignableGroups(string? q, ReviewStatus? status, int? cohortId, int? programId, int page = 0, int pageSize = 10)
    {
        var (groups, count) = await _reviewService.GetGroupsNeedingReviewAsync(q, status, cohortId, programId, page, pageSize);
        var summary = await _reviewService.GetReviewSummaryAsync();

        var programs = await _cohortService.GetProgramSelectItemsAsync();
        var cohorts = await _cohortService.CohortsAsOptionItemAsync();

        ViewBag.Search = q;
        ViewBag.Status = status;
        ViewBag.CohortId = cohortId;
        ViewBag.ProgramId = programId;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = count;
        ViewBag.Summary = summary;
        ViewBag.Programs = new SelectList(programs, "Id", "Name");
        ViewBag.Cohorts = new SelectList(cohorts, "Id", "Name");

        return View(groups);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Give(MentorReviewCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var mentorId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            await _reviewService.GiveReviewAsync(mentorId, dto);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                return Ok(new { message = "Review given successfully!" });
            TempData["SuccessMessage"] = "Review given successfully!";
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
    public async Task<IActionResult> GetStudents(string? q)
    {
        var students = await _cohortService.GetStudentSelectItemsAsync(q);
        return Json(students);
    }

    [HttpPost]
    public async Task<IActionResult> AssignReview([FromBody] MentorAssignReviewDTO dto)
    {
        if (dto == null || dto.GroupId == Guid.Empty || dto.ReviewerId == Guid.Empty)
            return BadRequest(new { message = "Invalid data." });

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var mentorId))
            return Unauthorized();

        try
        {
            await _reviewService.AssignReviewAsync(mentorId, dto);
            return Ok(new { message = "Review assigned successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        try
        {
            var reviewDto = await _reviewService.GetReviewDetailAsync(id);
            return View(reviewDto);
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> DetailByGroup(Guid groupId)
    {
        try
        {
            var reviewDto = await _reviewService.GetReviewDetailForGroupAsync(groupId);
            ViewBag.IsDetailByGroup = true;
            return View("Detail", reviewDto);
        }
        catch
        {
            return RedirectToAction(nameof(AssignableGroups));
        }
    }
}