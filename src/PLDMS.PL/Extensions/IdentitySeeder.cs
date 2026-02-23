using Microsoft.AspNetCore.Identity;
using PLDMS.Core.Entities;

namespace PLDMS.PL.Extensions;

public class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var config = services.GetRequiredService<IConfiguration>();

        var adminSection = config.GetSection("AdminUser");

        var email = adminSection["Email"];
        var password = adminSection["Password"];
        var fullName = adminSection["FullName"];

        const string roleName = "Admin";

        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                FullName = fullName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, roleName);
        }
    }

}