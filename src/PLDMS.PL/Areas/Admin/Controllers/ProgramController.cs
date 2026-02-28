using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs.ProgramDTOs;
using PLDMS.BL.Services.Abstractions;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProgramController : Controller
{
    private readonly IProgramService _programService;
    public ProgramController(IProgramService programService)
    {
        _programService = programService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string? q)
    {
        try
        {  
            var programs = await _programService.ProgramsAsItemAsync(q ?? "");
        
            ViewBag.CurrentSearch = q ?? "";
        
            return View(programs);
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong!");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProgramFormDTO dto)
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
            await _programService.CreateAsync(dto);
            await _programService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    
    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProgramFormDTO dto, int id)
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
            await _programService.UpdateAsync(id, dto);
            await _programService.SaveChangesAsync();
        
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
            await _programService.DeleteAsync(id);
            await _programService.SaveChangesAsync();
        
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
            await _programService.SoftDeleteAsync(id);
            await _programService.SaveChangesAsync();
        
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
            await _programService.RevertSoftDeleteAsync(id);
            await _programService.SaveChangesAsync();
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}