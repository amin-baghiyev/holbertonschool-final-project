using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.Property(s => s.RepositoryUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.CommitHash)
            .HasMaxLength(100);

        builder.Property(s => s.BranchName)
            .HasMaxLength(100);
    }
}