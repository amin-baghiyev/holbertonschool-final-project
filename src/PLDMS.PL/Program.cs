using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PLDMS.BL;
using PLDMS.Core.Entities;
using PLDMS.DL;
using PLDMS.DL.Contexts;
using PLDMS.PL.Common;
using PLDMS.PL.Extensions;
using PLDMS.PL.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL")
    )
);

builder.Services.AddControllersWithViews(opt =>
{
    opt.ModelValidatorProviders.Clear();
    opt.Filters.Add<GlobalExceptionFilter>();
});

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

builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();

builder.Services.AddDLServices();
builder.Services.AddBLServices();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/";
    opt.AccessDeniedPath = "/";
    opt.LogoutPath = "/";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    await IdentitySeeder.SeedAsync(services);
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "areas",
    "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute("default", "{controller=Account}/{action=Index}/{id?}");

app.Run();