using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.PL.Areas.Admin.ViewModels;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CohortController : Controller
{
    private readonly ICohortService _cohortService;
    private readonly IProgramService _programService;

    public CohortController(ICohortService cohortService, IProgramService programService)
    {
        _cohortService = cohortService;
        _programService = programService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCohortId(int id)
    {
        var students = await _cohortService.GetStudentSelectItemsByCohortIdAsync(id);
        return Json(students);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetStudents(string? q)
    {
        var students = await _cohortService.GetStudentSelectItemsAsync(q);
        return Json(students);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddStudentsToCohort([FromBody] AddStudentsRequest request)
    {
        await _cohortService.SyncStudentsInCohortAsync(request.CohortId, request.StudentIds);
        await _cohortService.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPrograms()
    {
        var programs = await _cohortService.GetProgramSelectItemsAsync();
        return Json(programs);
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, bool onlyActive = true, int page = 0, int pageSize = 10)
    {
        pageSize = pageSize switch
        {
            5 => 5,
            10 => 10,
            15 => 15,
            20 => 20,
            _ => 10
        };
        
        var (cohorts, totalCount) = await _cohortService.CohortsAsTableItemAsync(q ?? "", onlyActive, page, pageSize);

        var vm = new CohortVM
        {
            Cohorts = cohorts,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            Search = q ?? ""
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CohortFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _cohortService.CreateAsync(dto);
        await _cohortService.SaveChangesAsync();
        
        return Created();
    }


    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CohortFormDTO dto, int id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _cohortService.UpdateAsync(id, dto);
        await _cohortService.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _cohortService.SoftDeleteAsync(id);
        await _cohortService.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        await _cohortService.RevertSoftDeleteAsync(id);
        await _cohortService.SaveChangesAsync();
        return Ok();
    }
}