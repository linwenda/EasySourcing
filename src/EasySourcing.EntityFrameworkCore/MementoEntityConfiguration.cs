using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasySourcing.EntityFrameworkCore;

public class MementoEntityConfiguration:IEntityTypeConfiguration<MementoEntity>
{
    public void Configure(EntityTypeBuilder<MementoEntity> builder)
    {
        builder.ToTable("MementoStore");
        builder.HasKey(p => new
        {
            p.SourcedId,
            p.Version
        });
    }
}