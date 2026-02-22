using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class TaskLanguageConfiguration : IEntityTypeConfiguration<TaskLanguage>
{
    public void Configure(EntityTypeBuilder<TaskLanguage> builder)
    {
        builder.HasKey(tl => new { tl.TaskId, tl.ProgrammingLanguage });

        builder.HasOne(tl => tl.Task)
            .WithMany(t => t.TaskLanguages)
            .HasForeignKey(tl => tl.TaskId);
    }
}