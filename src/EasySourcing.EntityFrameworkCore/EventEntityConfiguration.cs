using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasySourcing.EntityFrameworkCore;

public class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("EventStore");
        builder.HasKey(p => new
        {
            p.SourcedId,
            p.Version
        });
    }
}