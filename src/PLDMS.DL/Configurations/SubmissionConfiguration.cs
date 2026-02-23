using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.Property(s => s.CommitHash)
            .HasMaxLength(12);

        var boolArrayToIntArrayConverter = new ValueConverter<bool[], int[]>(
            v => v.Select(b => b ? 1 : 0).ToArray(),
            v => v.Select(i => i == 1).ToArray()
        );

        var boolArrayComparer = new ValueComparer<bool[]>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray()
        );

        builder.Property(s => s.Tests)
            .HasConversion(boolArrayToIntArrayConverter)
            .HasColumnType("integer[]")
            .Metadata.SetValueComparer(boolArrayComparer);

        builder.Property(s => s.ProgrammingLanguage)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasOne(s => s.Group)
            .WithMany(g => g.Submissions)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Task)
            .WithMany()
            .HasForeignKey(s => s.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}