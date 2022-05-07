using EasySourcing.Abstraction;
using EasySourcing.EntityFrameworkCore.Tests.SeedWork;
using EasySourcing.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasySourcing.EntityFrameworkCore.Tests;

public class DependencyInjectionTests : TestBase
{
    [Fact]
    public void CanGetServices()
    {
        var eventStore = ServiceProvider.GetService<IEventStore>();
        Assert.NotNull(eventStore);

        var mementoStore = ServiceProvider.GetService<IMementoStore>();
        Assert.NotNull(mementoStore);

        var eventSourcedRepository = ServiceProvider.GetService<IEventSourcedRepository<Post>>();
        Assert.NotNull(eventSourcedRepository);

        var projector = ServiceProvider.GetService<IProjector>();
        Assert.NotNull(projector);
    }

    [Fact]
    public void CanGetProjectorHandlers()
    {
        var postProjectorForCreatedEvent = ServiceProvider.GetService<IProjectorHandler<PostCreatedEvent>>();
        Assert.NotNull(postProjectorForCreatedEvent);

        var postProjectorForEditedEvent = ServiceProvider.GetService<IProjectorHandler<PostEditedEvent>>();
        Assert.NotNull(postProjectorForEditedEvent);
    }
}