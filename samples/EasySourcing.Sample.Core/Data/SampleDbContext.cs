using EasySourcing.EntityFrameworkCore;
using EasySourcing.Sample.Core.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EasySourcing.Sample.Core.Data;

public class SampleDbContext : DbContext, IEventSourcingDbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MementoEntityConfiguration());
        
        //ReadModels
        modelBuilder.Entity<PostDetail>(b =>
        {
            b.ToTable("Posts");
            
            b.HasKey(p => p.Id);
            b.Property(p => p.Title).HasMaxLength(256);
        });
        
        modelBuilder.Entity<PostHistory>(b =>
        {
            b.ToTable("PostHistories");
            
            b.HasKey(p => p.Id);
            b.Property(p => p.Title).HasMaxLength(256);
        });
    }
}