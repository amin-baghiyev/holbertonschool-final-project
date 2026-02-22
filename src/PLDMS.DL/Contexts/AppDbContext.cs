using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PLDMS.Core.Entities;
using Task = PLDMS.Core.Entities.Task;

namespace PLDMS.DL.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    DbSet<Cohort> Cohorts { get; set; }
    DbSet<Group> Groups { get; set; }
    DbSet<Program> Programs { get; set; }
    DbSet<Review> Reviews { get; set; }
    DbSet<Session> Sessions { get; set; }
    DbSet<SessionTask> SessionTasks { get; set; }
    DbSet<Student> Students { get; set; }
    DbSet<StudentCohort> StudentCohorts { get; set; }
    DbSet<StudentGroup> StudentGroups { get; set; }
    DbSet<Submission> Submissions { get; set; }
    DbSet<Task> Tasks { get; set; }
    DbSet<TaskLanguage> TaskLanguages { get; set; }
    DbSet<TestCase> TestCases { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}