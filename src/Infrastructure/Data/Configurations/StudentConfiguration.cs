using Itishnik.Domain.Entities;
using Itishnik.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasBaseType<User>();
        builder.Property(s => s.EducationalProgram)
            .IsRequired()
            .HasMaxLength(255);
    }
}
