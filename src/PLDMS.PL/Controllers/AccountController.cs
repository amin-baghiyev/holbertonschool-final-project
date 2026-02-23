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

        await _service.LoginAsync(dto);
        return Redirect(returnUrl ?? (User.IsInRole(UserRole.Admin.ToString()) ? "/admin" : "/"));
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _service.LogoutAsync();
        return Redirect("/");
    }
}