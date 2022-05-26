using System;
using EasySourcing.DependencyInjection;
using EasySourcing.EntityFrameworkCore.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasySourcing.EntityFrameworkCore.Tests.SeedWork;

[Collection("Sequential")]
public class TestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider;

    protected TestBase()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(x => x.UseInMemoryDatabase("di_test"));

        services.Configure<EventSourcingOptions>(options =>
        {
            options.TakeEachSnapshotVersion = 3;
        });

        services.AddEventSourcing(builder =>
        {
            builder.UseEfCoreStore<TestDbContext>();
        });

        services.AddProjection(typeof(TestBase).Assembly);

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.GetRequiredService<TestDbContext>().Database.EnsureDeleted();
    }
}