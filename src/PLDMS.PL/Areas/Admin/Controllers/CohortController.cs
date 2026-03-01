using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs.CohortDTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.PL.Areas.Admin.ViewModels;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CohortController : Controller
{
    private readonly ICohortService _cohortService;
    public CohortController(ICohortService cohortService)
    {
        _cohortService = cohortService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPrograms()
    {
        try
        {
            var programs = await _cohortService.GetProgramSelectItemsAsync();
            return Json(programs);
        }
        catch (Exception e)
        {
            return BadRequest("Something went wrong!");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string? q, int page, int pageSize = 10)
    {
        try
        {
            pageSize = pageSize switch
            {
                5 => 5,
                10 => 10,
                15 => 15,
                20 => 20,
                _ => 10
            };
            
            var (cohorts, totalCount) = await _cohortService.CohortsAsTableItemAsync(q ?? "", page, pageSize);
        
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
        catch (Exception)
        {
            return BadRequest("Something went wrong!");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CohortFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        try
        {
            await _cohortService.CreateAsync(dto);
            await _cohortService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    
    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CohortFormDTO dto, int id)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        try
        {
            await _cohortService.UpdateAsync(id, dto);
            await _cohortService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _cohortService.DeleteAsync(id);
            await _cohortService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        try
        {
            await _cohortService.SoftDeleteAsync(id);
            await _cohortService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        try
        {
            await _cohortService.RevertSoftDeleteAsync(id);
            await _cohortService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}