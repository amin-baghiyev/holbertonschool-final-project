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

        services.AddSingleton(sp =>
        {
            var client = new GitHubClient(new ProductHeaderValue("HolbertonPLDSessions"))
            {
                Credentials = new Credentials(configuration["GitHub:Token"])
            };

            return client;
        });
        services.AddScoped<GitHubService>();

        services.AddHttpClient<IJudgeService, JudgeService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:2358");
        });

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProgramService, ProgramService>();
        services.AddScoped<ICohortService, CohortService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IMentorService, MentorService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReviewService, ReviewService>();
    }
}