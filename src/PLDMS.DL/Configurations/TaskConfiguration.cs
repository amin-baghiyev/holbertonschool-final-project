using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = PLDMS.Core.Entities.Task;

namespace PLDMS.DL.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        builder.HasMany(t => t.Submissions)
            .WithOne(s => s.Task)
            .HasForeignKey(s => s.TaskId);

        builder.HasMany(t => t.TestCases)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId);
    }
}