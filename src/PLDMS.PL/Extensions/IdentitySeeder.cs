using Microsoft.AspNetCore.Identity;
using PLDMS.Core.Entities;

namespace PLDMS.PL.Extensions;

public class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var config = services.GetRequiredService<IConfiguration>();

        var seedSection = config.GetSection("SeedUsers");

        await SeedUserAsync("Admin", seedSection.GetSection("Admin"), userManager, roleManager);
        await SeedUserAsync("Student", seedSection.GetSection("Student"), userManager, roleManager);
        await SeedUserAsync("Mentor", seedSection.GetSection("Mentor"), userManager, roleManager);
    }

    private static async Task SeedUserAsync(
        string roleName,
        IConfigurationSection section,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var email = section["Email"];
        var password = section["Password"];
        var fullName = section["FullName"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

        var user = await userManager.FindByEmailAsync(email);

        if (user != null) return;

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