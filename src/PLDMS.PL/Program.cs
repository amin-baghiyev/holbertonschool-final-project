using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.Core.Entities;
using PLDMS.DL;
using PLDMS.DL.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL")
    )
);

builder.Services.AddControllersWithViews(opt => opt.ModelValidatorProviders.Clear());

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 10;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDLServices();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/login";
    opt.AccessDeniedPath = "/";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Login}/{action=Index}/{id?}");

app.Run();