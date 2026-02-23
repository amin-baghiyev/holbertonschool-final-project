using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SessionExerciseConfiguration : IEntityTypeConfiguration<SessionExercise>
{
    public void Configure(EntityTypeBuilder<SessionExercise> builder)
    {
        builder.HasKey(st => new { st.SessionId, st.ExerciseId });

        builder.HasOne(st => st.Session)
            .WithMany(s => s.Exercises)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Exercise)
            .WithMany()
            .HasForeignKey(st => st.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}