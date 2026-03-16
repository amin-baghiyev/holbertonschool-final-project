using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs;
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
        var programs = await _programService.ProgramsAsItemAsync(q ?? "");
        ViewBag.CurrentSearch = q ?? "";
        return View(programs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProgramFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _programService.CreateAsync(dto);
        await _programService.SaveChangesAsync();
        return Ok();
    }


    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProgramFormDTO dto, int id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _programService.UpdateAsync(id, dto);
        await _programService.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _programService.DeleteAsync(id);
        await _programService.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _programService.SoftDeleteAsync(id);
        await _programService.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        await _programService.RevertSoftDeleteAsync(id);
        await _programService.SaveChangesAsync();
        return Ok();
    }
}