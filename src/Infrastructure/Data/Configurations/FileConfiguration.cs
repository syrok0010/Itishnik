using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Itishnik.Domain.Entities.File;

namespace Itishnik.Infrastructure.Data.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Path)
            .IsRequired()
            .HasMaxLength(500);
    }
}
