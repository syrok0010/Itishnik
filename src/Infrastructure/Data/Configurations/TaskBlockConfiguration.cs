using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class TaskBlockConfiguration : IEntityTypeConfiguration<TaskBlock>
{
    public void Configure(EntityTypeBuilder<TaskBlock> builder)
    {
        builder.HasKey(tb => tb.Id);
        builder.Property(tb => tb.Name)
            .HasMaxLength(255);
        builder.HasMany(tb => tb.Tasks)
            .WithMany();
        builder.PrimitiveCollection(tb => tb.Weights);
        builder.HasOne(tb => tb.Course)
            .WithMany(c => c.TaskBlocks)
            .HasForeignKey(tb => tb.CourseId);
    }
}
