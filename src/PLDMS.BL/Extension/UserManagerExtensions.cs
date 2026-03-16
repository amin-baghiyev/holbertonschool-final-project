using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;

namespace PLDMS.BL.Extension;

public static class UserManagerExtensions
{
    public static async Task ThrowIfInRoleAsync<T>(this UserManager<T> manager, string email, UserRole role) where T : class
    {
        if (!await manager.Users
                .Where(u => 
                    EF.Property<string>(u, nameof(AppUser.Email)) == email &&
                    EF.Property<UserRole>(u, nameof(AppUser.Role)) == role)
                .AnyAsync()
            )
        {
            throw new BaseException($"This {role} already exists with this email: {email}");
        }
    }
}