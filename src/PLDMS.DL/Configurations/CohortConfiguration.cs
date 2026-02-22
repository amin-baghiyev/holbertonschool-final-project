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

        builder.Property(c => c.StudentCount)
            .HasDefaultValue(0);

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.HasMany(c => c.Sessions)
            .WithOne(s => s.Cohort)
            .HasForeignKey(s => s.CohortId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}