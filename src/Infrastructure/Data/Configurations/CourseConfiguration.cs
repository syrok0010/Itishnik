using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.Description)
            .IsRequired();
        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.Courses);
        builder.HasMany(x => x.Students)
            .WithMany(x => x.Courses);
    }
}
