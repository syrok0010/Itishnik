using Itishnik.Domain.Entities;
using Itishnik.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasBaseType<ApplicationUser>();
        builder.Property(s => s.EducationalProgram)
            .HasMaxLength(255);
    }
}
