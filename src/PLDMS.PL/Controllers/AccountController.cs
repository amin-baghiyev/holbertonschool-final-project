using Microsoft.AspNetCore.Mvc;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;

namespace PLDMS.PL.Controllers;

public class AccountController : Controller
{
    readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole(UserRole.Admin.ToString()))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (User.IsInRole(UserRole.Mentor.ToString()))
                return RedirectToAction("Index", "Session", new { area = "Mentor" });

            if (User.IsInRole(UserRole.Student.ToString()))
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromBody] UserLoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _service.LoginAsync(dto);

        var redirectUrl = user.Role switch
        {
            UserRole.Admin => Url.Action("Index", "Dashboard", new { area = "Admin" }),
            UserRole.Mentor => Url.Action("Index", "Session", new { area = "Mentor" }),
            UserRole.Student => Url.Action("Index", "Dashboard", new { area = "Student" }),
            _ => Url.Action("Index", "Account")
        };

        return Ok(new { redirectUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _service.LogoutAsync();
        return Redirect("/");
    }
}