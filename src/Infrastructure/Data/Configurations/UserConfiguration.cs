using Itishnik.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Surname)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Patronymic)
            .HasMaxLength(100);
    }
}
