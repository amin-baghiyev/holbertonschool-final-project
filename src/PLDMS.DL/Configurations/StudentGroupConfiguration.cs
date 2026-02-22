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
            .HasForeignKey(sg => sg.StudentId);

        builder.HasOne(sg => sg.Group)
            .WithMany()
            .HasForeignKey(sg => sg.GroupId);
    }
}