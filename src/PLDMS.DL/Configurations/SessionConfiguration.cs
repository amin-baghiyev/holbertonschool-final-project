using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.RepositoryUrl)
            .IsRequired();

        builder.Property(s => s.StudentCountPerGroup)
            .HasDefaultValue(2);

        builder.Property(s => s.TotalStudentCount)
            .HasDefaultValue(0);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.StartDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.EndDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasOne(s => s.Cohort)
            .WithMany(c => c.Sessions)
            .HasForeignKey(s => s.CohortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Groups)
            .WithOne(g => g.Session)
            .HasForeignKey(g => g.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}