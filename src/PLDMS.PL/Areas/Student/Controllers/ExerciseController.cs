using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.DL.Repositories.Abstractions;
using PLDMS.Core.Entities;
using System.Security.Claims;

namespace PLDMS.PL.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public class ExerciseController : Controller
{
    private readonly IExerciseService _exerciseService;
    private readonly ISubmissionService _submissionService;
    private readonly IRepository<Group> _groupRepository;

    public ExerciseController(IExerciseService exerciseService, ISubmissionService submissionService, IRepository<Group> groupRepository)
    {
        _exerciseService = exerciseService;
        _submissionService = submissionService;
        _groupRepository = groupRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Details(long exerciseId, Guid groupId)
    {
        var dto = await _exerciseService.ExerciseByIdForStudentAsync(exerciseId);
        
        ViewBag.GroupId = groupId;

        // Load session info for header countdown
        var group = await _groupRepository.GetOneAsync(
            predicate: g => g.Id == groupId,
            includes: q => q.Include(g => g.Session),
            isTracking: false);

        if (group?.Session != null)
        {
            ViewBag.SessionName = group.Session.Name;
            ViewBag.SessionEndDate = group.Session.EndDate;
            ViewBag.SessionId = group.Session.Id;
        }

        ViewBag.Submissions = await _submissionService.GetSubmissionsByGroupAsync(groupId, exerciseId);
        
        var lastCode = await _submissionService.GetLastSubmittedCodeAsync(groupId, exerciseId);
        ViewBag.LastSubmittedCode = lastCode;

        if (!string.IsNullOrEmpty(lastCode))
        {
            var submissions = (IEnumerable<PLDMS.BL.DTOs.SubmissionListItemDTO>)ViewBag.Submissions;
            var latest = submissions.FirstOrDefault();
            if (latest != null)
            {
                ViewBag.LastSubmittedLanguage = (int)latest.ProgrammingLanguage;
            }
        }

        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetTestCases(long id)
    {
        var exercise = await _exerciseService.ExerciseByIdForStudentAsync(id);
        return Json(exercise.TestCases);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitCode([FromBody] CodeSubmissionDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            var result = await _submissionService.SubmitCodeAsync(studentId, dto);
            return Json(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunCode([FromBody] CodeSubmissionDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            return RedirectToAction("Index", "Dashboard");

        try
        {
            var result = await _submissionService.RunCodeAsync(studentId, dto);
            return Json(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
