using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.Property(tc => tc.Input)
            .IsRequired();

        builder.Property(tc => tc.Output)
            .IsRequired();

        builder.Property(tc => tc.IsDeleted)
            .HasDefaultValue(false);
    }
}