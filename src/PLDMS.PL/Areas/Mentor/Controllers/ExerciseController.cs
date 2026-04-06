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
public class ExerciseController : Controller
{
    private readonly IExerciseService _exerciseService;
    private readonly IProgramService _programService;

    public ExerciseController(IExerciseService exerciseService, IProgramService programService)
    {
        _exerciseService = exerciseService;
        _programService = programService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int? programId, ExerciseDifficulty? difficulty, bool onlyActive = true, int page = 0, int pageSize = 10)
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

        var programsFilter = programId.HasValue ? new List<int> { programId.Value } : null;

        var (exercises, totalCount) = await _exerciseService.ExercisesAsTableItemAsync(
            q: q,
            programs: programsFilter,
            languages: null,
            difficulty: difficulty,
            onlyActive: onlyActive,
            page: page,
            count: pageSize
        );

        await LoadDropdownDataAsync(programId);

        var vm = new ExerciseVM
        {
            Exercises = [.. exercises],
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            Search = q ?? ""
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> GetPrograms()
    {
        var programs = await _programService.ProgramsAsOptionItemAsync();
        return Json(programs);
    }

    [HttpGet]
    public async Task<IActionResult> GetForEdit(long id)
    {
        var exercise = await _exerciseService.ExerciseByIdForEditAsync(id);
        return Json(exercise);
    }

    [HttpGet]
    public async Task<IActionResult> GetTestCases(long id)
    {
        var exercise = await _exerciseService.ExerciseByIdForEditAsync(id);
        return Json(exercise.TestCases);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] ExerciseFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _exerciseService.CreateAsync(dto);
        return Created();
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(long id, [FromBody] ExerciseFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _exerciseService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _exerciseService.SoftDeleteAsync(id);
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(long id)
    {
        await _exerciseService.RecoverAsync(id);
        return Ok();
    }

    private async Task LoadDropdownDataAsync(int? selectedProgramId = null)
    {
        var programs = await _programService.ProgramsAsOptionItemAsync();
        ViewBag.Programs = new SelectList(programs, "Id", "Name", selectedProgramId);
    }
}