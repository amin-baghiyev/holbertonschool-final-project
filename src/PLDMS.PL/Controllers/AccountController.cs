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
    public async Task<IActionResult> Index(UserLoginDTO dto, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var user = await _service.LoginAsync(dto);
        
        return user.Role switch
        {
            UserRole.Admin => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
            UserRole.Mentor => RedirectToAction("Index", "Session", new { area = "Mentor" }),
            UserRole.Student => RedirectToAction("Index", "Dashboard", new { area = "Student" }),
            _ => RedirectToAction("Index", "Account")
        };
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _service.LogoutAsync();
        return Redirect("/");
    }
}