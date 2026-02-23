using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class StudentCohortConfiguration : IEntityTypeConfiguration<StudentCohort>
{
    public void Configure(EntityTypeBuilder<StudentCohort> builder)
    {
        builder.HasKey(sc => new { sc.StudentId, sc.CohortId });

        builder.HasOne(sc => sc.Student)
            .WithMany()
            .HasForeignKey(sc => sc.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Cohort)
            .WithMany()
            .HasForeignKey(sc => sc.CohortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
    }
}