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
    private readonly IProgramService _programService;

    public SessionController(
        ISessionService sessionService,
        ICohortService cohortService,
        IExerciseService exerciseService,
        IProgramService programService)
    {
        _sessionService = sessionService;
        _cohortService = cohortService;
        _exerciseService = exerciseService;
        _programService = programService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int? cohortId, int? programId, DateTime? startDate, DateTime? endDate, SessionStatus? status, int page = 0, int pageSize = 10)
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
            cohortId,
            programId,
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
            Status = status,
            CohortId = cohortId,
            ProgramId = programId
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

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        try
        {
            var session = await _sessionService.SessionByIdAsync(id);
            return View(session);
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
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

        var programs = await _programService.ProgramsAsOptionItemAsync();
        ViewBag.Programs = new SelectList(programs, nameof(ProgramOptionItemDTO.Id), nameof(ProgramOptionItemDTO.Name));
    }

    [HttpGet]
    public async Task<IActionResult> SearchExercises(string? q, ExerciseDifficulty? difficulty, int? programId, ProgrammingLanguage? language)
    {
        var languagesParam = language.HasValue ? new[] { language.Value } : null;
        var programsParam = programId.HasValue ? new[] { programId.Value } : null;

        var (exercises, _) = await _exerciseService.ExercisesAsTableItemAsync(
            q, programsParam, languagesParam, difficulty, onlyActive: true, page: 0, count: 50);

        var result = exercises.Select(e => new
        {
            id = e.Id,
            name = e.Name,
            difficulty = e.Difficulty.ToString(),
            programName = e.ProgramName,
            languages = e.Languages.Select(l => l.ToString()).ToList()
        });

        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetExercisesBySessionId(Guid id)
    {
        try
        {
            var exercises = await _sessionService.GetExercisesBySessionIdAsync(id);
            var result = exercises.Select(e => new
            {
                id = e.Id,
                name = e.Name,
                difficulty = e.Difficulty.ToString(),
                programName = e.ProgramName,
                languages = e.Languages.Select(l => l.ToString()).ToList()
            });

            return Json(result);
        }
        catch (Exception)
        {
            return Json(new object[] { });
        }
    }
}