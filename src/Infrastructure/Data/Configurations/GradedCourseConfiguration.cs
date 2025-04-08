using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class GradedCourseConfiguration : IEntityTypeConfiguration<GradedCourse>
{
    public void Configure(EntityTypeBuilder<GradedCourse> builder)
    {
        builder.HasKey(gc => gc.Id);
        builder.HasOne(gc => gc.Course)
            .WithMany(c => c.GradedCourses)
            .HasForeignKey(gc => gc.CourseId);
        builder.HasOne(gc => gc.Student)
            .WithMany(s => s.GradedCourses)
            .HasForeignKey(gc => gc.StudentId);
    }
}
