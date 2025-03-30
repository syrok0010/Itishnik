using Itishnik.Domain.Entities;
using Itishnik.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.UseTptMappingStrategy();
        builder.Property(u => u.Name)
            .HasMaxLength(100);
        builder.Property(u => u.Surname)
            .HasMaxLength(100);
        builder.Property(u => u.Patronymic)
            .HasMaxLength(100);
    }
}
