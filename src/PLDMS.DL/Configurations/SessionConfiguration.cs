using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.StudentCount)
            .HasDefaultValue(0);

        builder.HasMany(s => s.Groups)
            .WithOne(g => g.Session)
            .HasForeignKey(g => g.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}