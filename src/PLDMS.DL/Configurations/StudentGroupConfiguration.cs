using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLDMS.Core.Entities;

namespace PLDMS.DL.Configurations;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.HasKey(sg => new { sg.StudentId, sg.GroupId });

        builder.HasOne(sg => sg.Student)
            .WithMany()
            .HasForeignKey(sg => sg.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sg => sg.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(sg => sg.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}