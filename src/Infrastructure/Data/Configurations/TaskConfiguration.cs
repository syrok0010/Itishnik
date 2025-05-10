using Itishnik.Domain.Entities;
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
            .HasMaxLength(255);
        builder.Property(t => t.Text)
            .HasMaxLength(5000);
        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId);
        builder.HasMany<Solution>()
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId);
        builder.HasMany(x => x.Tags)
            .WithMany();
        builder
            .HasOne(x => x.PreviousVersion)
            .WithOne();
        builder
            .HasOne(x => x.FirstVersion)
            .WithMany();
    }
}
