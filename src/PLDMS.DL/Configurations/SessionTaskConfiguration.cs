using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SessionTaskConfiguration : IEntityTypeConfiguration<SessionTask>
{
    public void Configure(EntityTypeBuilder<SessionTask> builder)
    {
        builder.HasKey(st => new { st.SessionId, st.TaskId });

        builder.HasOne(st => st.Session)
            .WithMany(s => s.Tasks)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Task)
            .WithMany()
            .HasForeignKey(st => st.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}