using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasySourcing.EntityFrameworkCore;

public class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
{
    private readonly string _tableName;

    public EventEntityConfiguration()
    {
        _tableName = "EventStore";
    }

    public EventEntityConfiguration(string tableName)
    {
        _tableName = !string.IsNullOrEmpty(tableName) ? tableName : "EventStore";
    }

    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(p => new
        {
            p.SourcedId,
            p.Version
        });
    }
}