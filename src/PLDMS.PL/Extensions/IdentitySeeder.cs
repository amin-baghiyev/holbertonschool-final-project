using Microsoft.AspNetCore.Identity;
using PLDMS.Core.Entities;
using PLDMS.Core.Enums;

namespace PLDMS.PL.Extensions;

public class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        var seedSection = config.GetSection("SeedUsers");

        await SeedUserAsync(seedSection.GetSection("Admin"), userManager, UserRole.Admin);
        await SeedUserAsync(seedSection.GetSection("Mentor"), userManager, UserRole.Mentor);
        await SeedUserAsync(seedSection.GetSection("Student"), userManager, UserRole.Student);
    }

    private static async Task SeedUserAsync(IConfigurationSection section, UserManager<AppUser> userManager, UserRole role)
    {
        var email = section["Email"];
        var password = section["Password"];
        var fullName = section["FullName"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            return;

        var user = await userManager.FindByEmailAsync(email);

        if (user is not null) return;

        user = new AppUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = email,
            FullName = fullName,
            Role = role,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        await userManager.CreateAsync(user, password);
    }
}