using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Score)
            .IsRequired();

        builder.Property(r => r.Note)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasOne(r => r.Reviewer)
            .WithMany(u => u.ReviewsGiven)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.AssignedBy)
            .WithMany(u => u.ReviewsAssigned)
            .HasForeignKey(r => r.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Group)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}