using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Duration)
           .HasColumnType("interval")
           .IsRequired();

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
    }
}