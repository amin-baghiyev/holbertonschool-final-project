using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using PLDMS.BL.Services.Abstractions;
using PLDMS.BL.Services.Concretes;
using PLDMS.BL.Utilities;
using System.Reflection;

namespace PLDMS.BL;

public static class ConfigurationServices
{
    public static void AddBLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddSingleton(new GitHubClient(new ProductHeaderValue("HolbertonPLDSessions"))
        {
            Credentials = new Credentials(configuration["GitHub:Token"])
        });
        services.AddScoped<GitHubService>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProgramService, ProgramService>();
        services.AddScoped<ICohortService, CohortService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ISessionService, SessionService>();
    }
}