using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class TaskBlockEntryConfiguration : IEntityTypeConfiguration<TaskBlockEntry>
{
    public void Configure(EntityTypeBuilder<TaskBlockEntry> builder)
    {
        builder
            .HasOne(b => b.Task)
            .WithMany(b => b.TaskBlockEntries);
        builder
            .HasOne(b => b.TaskBlock)
            .WithMany(b => b.TasksEntries); 
    }
}
