using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.Difficulty)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(e => e.Program)
            .WithMany()
            .HasForeignKey(e => e.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.TestCases)
            .WithOne(tc => tc.Exercise)
            .HasForeignKey(tc => tc.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ExerciseLanguages)
            .WithOne(el => el.Exercise)
            .HasForeignKey(el => el.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}