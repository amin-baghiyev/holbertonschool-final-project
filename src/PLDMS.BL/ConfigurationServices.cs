using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using PLDMS.BL.Services.Abstractions;
using PLDMS.BL.Services.Concretes;
using System.Reflection;

namespace PLDMS.BL;

public static class ConfigurationServices
{
    public static void AddBLServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProgramService, ProgramService>();
    }
}