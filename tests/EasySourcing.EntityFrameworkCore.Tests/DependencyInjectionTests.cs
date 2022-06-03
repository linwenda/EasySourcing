using System;
using EasySourcing.Abstraction;
using EasySourcing.EntityFrameworkCore.Tests.SeedWork;
using EasySourcing.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasySourcing.EntityFrameworkCore.Tests;

[Collection("Sequential")]
public class DependencyInjectionTests : IClassFixture<EasySourcingFixture>
{
    private readonly IServiceProvider _serviceProvider;
    
    public DependencyInjectionTests(EasySourcingFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }
    
    [Fact]
    public void CanGetServices()
    {
        var eventStore = _serviceProvider.GetService<IEventStore>();
        Assert.NotNull(eventStore);

        var mementoStore = _serviceProvider.GetService<IMementoStore>();
        Assert.NotNull(mementoStore);

        var eventSourcedRepository = _serviceProvider.GetService<IEventSourcedRepository<Post>>();
        Assert.NotNull(eventSourcedRepository);

        var projector = _serviceProvider.GetService<IEventPublisher>();
        Assert.NotNull(projector);
    }

    [Fact]
    public void CanGetEventHandlers()
    {
        var handler1 = _serviceProvider.GetService<IEventHandler<PostCreatedEvent>>();
        Assert.NotNull(handler1);

        var handler2 = _serviceProvider.GetService<IEventHandler<PostEditedEvent>>();
        Assert.NotNull(handler2);
    }
}