using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PLDMS.Core.Entities;
using System.Reflection;
using Task = PLDMS.Core.Entities.Task;

namespace PLDMS.DL.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Cohort> Cohorts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Program> Programs { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<SessionTask> SessionTasks { get; set; }
    public DbSet<StudentCohort> StudentCohorts { get; set; }
    public DbSet<StudentGroup> StudentGroups { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskLanguage> TaskLanguages { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}