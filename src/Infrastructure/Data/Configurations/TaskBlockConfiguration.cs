using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class TaskBlockConfiguration : IEntityTypeConfiguration<TaskBlock>
{
    public void Configure(EntityTypeBuilder<TaskBlock> builder)
    {
        builder.HasKey(tb => tb.Id);
        builder.Property(tb => tb.Name).IsRequired().HasMaxLength(255);
        builder.HasOne(tb => tb.Course)
            .WithMany(c => c.TaskBlocks)
            .HasForeignKey(tb => tb.CourseId);
    }
}
