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
            .WithMany()
            .HasForeignKey(st => st.SessionId);

        builder.HasOne(st => st.Task)
            .WithMany()
            .HasForeignKey(st => st.TaskId);
    }
}