using Microsoft.Extensions.DependencyInjection;
using PLDMS.Core.Entities;
using PLDMS.DL.Repositories.Abstractions;
using PLDMS.DL.Repositories.Implementations;

namespace PLDMS.DL;

public static class ConfigurationServices
{
    public static void AddDLServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Cohort>, Repository<Cohort>>();
        services.AddScoped<IRepository<Group>, Repository<Group>>();
        services.AddScoped<IRepository<Program>, Repository<Program>>();
        services.AddScoped<IRepository<Review>, Repository<Review>>();
        services.AddScoped<IRepository<Session>, Repository<Session>>();
        services.AddScoped<IRepository<SessionExercise>, Repository<SessionExercise>>();
        services.AddScoped<IRepository<StudentCohort>, Repository<StudentCohort>>();
        services.AddScoped<IRepository<StudentGroup>, Repository<StudentGroup>>();
        services.AddScoped<IRepository<Submission>, Repository<Submission>>();
        services.AddScoped<IRepository<Exercise>, Repository<Exercise>>();
        services.AddScoped<IRepository<ExerciseLanguage>, Repository<ExerciseLanguage>>();
        services.AddScoped<IRepository<TestCase>, Repository<TestCase>>();
    }
}