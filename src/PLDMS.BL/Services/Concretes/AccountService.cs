using Microsoft.AspNetCore.Identity;
using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Services.Concretes;

public class AccountService : IAccountService
{
    readonly UserManager<AppUser> _userManager;
    readonly SignInManager<AppUser> _singInManager;

    public AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> singInManager)
    {
        _userManager = userManager;
        _singInManager = singInManager;
    }

    public async Task<AppUser> LoginAsync(UserLoginDTO dto)
    {
        AppUser user = await _userManager.FindByEmailAsync(dto.Email) ?? throw new BaseException("Credentials are wrong");

        SignInResult res = await _singInManager.PasswordSignInAsync(user, dto.Password, false, true);

        if (!res.Succeeded) throw new BaseException("Credentials are wrong");

        return user;
    }

    public async Task LogoutAsync()
    {
        await _singInManager.SignOutAsync();
    }
}