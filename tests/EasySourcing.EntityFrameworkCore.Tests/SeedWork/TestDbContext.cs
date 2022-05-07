using EasySourcing.TestFixtures;
using Microsoft.EntityFrameworkCore;

namespace EasySourcing.EntityFrameworkCore.Tests.SeedWork;

public class TestDbContext : DbContext, IEventSourcingDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MementoEntityConfiguration());

        modelBuilder.Entity<PostDetail>().HasKey(p => p.Id);
        modelBuilder.Entity<PostHistory>().HasKey(p => p.Id);
    }
}