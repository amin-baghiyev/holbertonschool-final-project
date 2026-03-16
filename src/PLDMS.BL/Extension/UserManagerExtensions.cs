using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL.Common;

namespace PLDMS.BL.Extension;

public static class UserManagerExtensions
{
    public static async Task ThrowIfInRoleAsync<T>(this UserManager<T> manager, string email, string role) where T : class
    {
        var users = await manager.Users
            .Where(u => EF.Property<string>(u, "Email") == email)
            .ToListAsync();

        foreach (var user in users)
        {
            if (await manager.IsInRoleAsync(user, role))
            {
                string userName = (user as dynamic).UserName;
                throw new BaseException($"This {role} already exists with username: {userName}.");
            }
        }
    }

}