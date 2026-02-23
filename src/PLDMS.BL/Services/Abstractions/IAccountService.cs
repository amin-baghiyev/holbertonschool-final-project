using PLDMS.BL.DTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface IAccountService
{
    Task LoginAsync(UserLoginDTO dto);
    Task LogoutAsync();
}