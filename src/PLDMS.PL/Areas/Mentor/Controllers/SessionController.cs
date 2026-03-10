using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;
using PLDMS.PL.Areas.Mentor.ViewModels;

namespace PLDMS.PL.Areas.Mentor.Controllers;

[Area("Mentor")]
[Authorize(Roles = nameof(UserRole.Mentor))]
public class SessionController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly ICohortService _cohortService;
    private readonly IExerciseService _exerciseService;

    public SessionController(
        ISessionService sessionService,
        ICohortService cohortService,
        IExerciseService exerciseService)
    {
        _sessionService = sessionService;
        _cohortService = cohortService;
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, DateTime? startDate, DateTime? endDate, SessionStatus? status, int page = 0, int pageSize = 10)
    {
        pageSize = pageSize switch
        {
            5 => 5,
            10 => 10,
            15 => 15,
            20 => 20,
            25 => 25,
            _ => 10
        };

        var (sessions, totalCount) = await _sessionService.SessionsAsTableItemAsync(
            q,
            startDate,
            endDate,
            status,
            page,
            count: pageSize
        );

        await LoadDropdownDataAsync();

        var viewModel = new SessionVM
        {
            Sessions = sessions,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            Search = q,
            Status = status
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetForEdit(Guid id)
    {
        try
        {
            var session = await _sessionService.SessionByIdForEditAsync(id);
            return Json(new { data = session });
        }
        catch (Exception ex)
        {
            return Json(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] SessionFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        await _sessionService.CreateAsync(dto);
        return Ok();
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromBody] SessionFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _sessionService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sessionService.DeleteAsync(id);
        return Ok();
    }

    private async Task LoadDropdownDataAsync()
    {
        var cohorts = await _cohortService.CohortsAsOptionItemAsync();
        ViewBag.Cohorts = new SelectList(cohorts, nameof(CohortOptionItemDTO.Id), nameof(CohortOptionItemDTO.Name));

        var exercises = await _exerciseService.ExercisesAsOptionItemAsync();
        ViewBag.Exercises = new SelectList(exercises, nameof(ExerciseAsOptionDTO.Id), nameof(ExerciseAsOptionDTO.Name));
    }
}