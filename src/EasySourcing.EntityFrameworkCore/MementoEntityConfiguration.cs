using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasySourcing.EntityFrameworkCore;

public class MementoEntityConfiguration : IEntityTypeConfiguration<MementoEntity>
{
    private readonly string _tableName;

    public MementoEntityConfiguration()
    {
        _tableName = "MementoStore";
    }

    public MementoEntityConfiguration(string tableName)
    {
        _tableName = !string.IsNullOrEmpty(tableName) ? tableName : "MementoStore";
    }

    public void Configure(EntityTypeBuilder<MementoEntity> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(p => new
        {
            p.SourcedId,
            p.Version
        });
    }
}