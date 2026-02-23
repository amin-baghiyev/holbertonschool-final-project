using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.TotalStudentCount)
            .HasDefaultValue(0);

        builder.HasOne(g => g.Session)
            .WithMany(s => s.Groups)
            .HasForeignKey(g => g.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Submissions)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}