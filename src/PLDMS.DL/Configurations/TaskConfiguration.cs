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

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(t => t.Difficulty)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(t => t.Program)
            .WithMany()
            .HasForeignKey(t => t.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.TestCases)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TaskLanguages)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}