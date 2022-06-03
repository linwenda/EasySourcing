using System;
using EasySourcing.DependencyInjection;
using EasySourcing.EntityFrameworkCore.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.EntityFrameworkCore.Tests.SeedWork;

public class EasySourcingFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public EasySourcingFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(x => x.UseInMemoryDatabase("di_test"));

        services.Configure<EventSourcingOptions>(options => options.TakeEachSnapshotVersion = 3);

        services.AddEasySourcing(typeof(EasySourcingFixture).Assembly,
            builder => builder.UseEfCoreStore<TestDbContext>());

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.GetRequiredService<TestDbContext>().Database.EnsureDeleted();
    }
}