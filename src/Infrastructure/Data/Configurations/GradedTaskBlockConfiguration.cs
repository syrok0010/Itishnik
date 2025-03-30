using Itishnik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itishnik.Infrastructure.Data.Configurations;

public class GradedTaskBlockConfiguration : IEntityTypeConfiguration<GradedTaskBlock>
{
    public void Configure(EntityTypeBuilder<GradedTaskBlock> builder)
    {
        builder.HasKey(gtb => gtb.Id);
        builder.HasOne(gtb => gtb.Student)
            .WithMany()
            .HasForeignKey(gtb => gtb.StudentId);
        builder.HasOne(gtb => gtb.TaskBlock)
            .WithMany()
            .HasForeignKey(gtb => gtb.TaskBlockId);
    }
}
