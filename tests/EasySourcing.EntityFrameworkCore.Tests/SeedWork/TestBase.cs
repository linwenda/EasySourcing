using System;
using EasySourcing.DependencyInjection;
using EasySourcing.EntityFrameworkCore.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.EntityFrameworkCore.Tests.SeedWork;

public class TestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider;

    protected TestBase()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(x => x.UseInMemoryDatabase("di_test"));

        services.AddEventSourcing(o => o.TakeEachSnapshotVersion = 3)
            .AddEfCoreStore<TestDbContext>()
            .AddProjection(typeof(TestBase).Assembly);

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.GetRequiredService<TestDbContext>().Database.EnsureDeleted();
    }
}