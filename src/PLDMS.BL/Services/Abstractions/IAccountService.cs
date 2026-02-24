using PLDMS.BL.DTOs;
using PLDMS.Core.Entities;

namespace PLDMS.BL.Services.Abstractions;

public interface IAccountService
{
    Task<AppUser> LoginAsync(UserLoginDTO dto);
    Task LogoutAsync();
}