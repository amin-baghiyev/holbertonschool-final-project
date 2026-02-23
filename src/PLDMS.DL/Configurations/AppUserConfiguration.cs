using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Ignore(u => u.PhoneNumber);
        builder.Ignore(u => u.PhoneNumberConfirmed);
        builder.Ignore(u => u.TwoFactorEnabled);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.HasMany(u => u.ReviewsGiven)
            .WithOne(r => r.Reviewer)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReviewsAssigned)
            .WithOne(r => r.AssignedBy)
            .HasForeignKey(r => r.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

    }
}