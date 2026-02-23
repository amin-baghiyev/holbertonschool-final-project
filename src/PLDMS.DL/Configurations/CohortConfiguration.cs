using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class CohortConfiguration : IEntityTypeConfiguration<Cohort>
{
    public void Configure(EntityTypeBuilder<Cohort> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.TotalStudentCount)
            .HasDefaultValue(0);

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(c => c.Program)
            .WithMany()
            .HasForeignKey(c => c.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Sessions)
            .WithOne(s => s.Cohort)
            .HasForeignKey(s => s.CohortId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}