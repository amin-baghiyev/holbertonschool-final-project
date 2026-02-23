using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class ExerciseLanguageConfiguration : IEntityTypeConfiguration<ExerciseLanguage>
{
    public void Configure(EntityTypeBuilder<ExerciseLanguage> builder)
    {
        builder.HasKey(tl => new { tl.ExerciseId, tl.ProgrammingLanguage });

        builder.HasOne(tl => tl.Exercise)
            .WithMany(t => t.ExerciseLanguages)
            .HasForeignKey(tl => tl.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}