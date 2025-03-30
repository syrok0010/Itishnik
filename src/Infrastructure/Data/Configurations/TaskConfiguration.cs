using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Infrastructure.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId);
    }
}
