using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.PL.Areas.Admin.ViewModels;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int page, int pageSize = 10)
    {
        pageSize = pageSize switch
        {
            5 => 5,
            10 => 10,
            15 => 15,
            20 => 20,
            _ => 10
        };

        var (students, totalCount) = await _studentService.StudentsAsTableItemAsync(q ?? "", page, pageSize);

        var vm = new StudentVM
        {
            Students = students,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            Search = q ?? ""
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentFormDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _studentService.CreateAsync(dto);
        await _studentService.SaveChangesAsync();

        return Ok();
    }


    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(StudentFormDTO dto, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _studentService.UpdateAsync(id, dto);
        await _studentService.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _studentService.DeleteAsync(id);
        await _studentService.SaveChangesAsync();

        return Ok();
    }
}