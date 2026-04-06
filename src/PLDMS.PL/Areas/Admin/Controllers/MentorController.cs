using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs.MentorDTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.PL.Areas.Admin.ViewModels;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class MentorController : Controller
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
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

        var (mentors, totalCount) = await _mentorService.MentorsAsTableItemAsync(q ?? "", onlyActive, page, pageSize);

        var vm = new MentorVM
        {
            Mentors = mentors,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            Search = q ?? ""
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MentorFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _mentorService.CreateAsync(dto);
        await _mentorService.SaveChangesAsync();
        return Ok();
    }


    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(MentorFormDTO dto, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _mentorService.UpdateAsync(id, dto);
        await _mentorService.SaveChangesAsync();

        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _mentorService.SoftDeleteAsync(id);
        await _mentorService.SaveChangesAsync();

        return Ok();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _mentorService.RecoverAsync(id);
        await _mentorService.SaveChangesAsync();

        return Ok();
    }
}