using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySourcing.Abstraction;
using EasySourcing.EntityFrameworkCore.Tests.SeedWork;
using EasySourcing.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace EasySourcing.EntityFrameworkCore.Tests;

[Collection("Sequential")]
public class EntityFrameworkCoreStoreTests : IClassFixture<EasySourcingFixture>
{
    private readonly IServiceProvider _serviceProvider;
    
    public EntityFrameworkCoreStoreTests(EasySourcingFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }
    
    [Fact]
    public async Task CanSaveEvents()
    {
        var eventStore = _serviceProvider.GetRequiredService<IEventStore>();

        var sourcedId = Guid.NewGuid();

        var expectedEvents = new List<IVersionedEvent>
        {
            new PostCreatedEvent(sourcedId, 1, "test", "test", Guid.NewGuid()),
            new PostEditedEvent(sourcedId, 2, "test", "test")
        };

        await eventStore.SaveAsync(expectedEvents);

        var actualEvents = await eventStore.LoadAsync(sourcedId, 0);

        Assert.Equal(2, actualEvents.Count());

        foreach (var expectedEvent in expectedEvents)
        {
            var actualEvent = actualEvents.Single(e =>
                e.SourcedId == expectedEvent.SourcedId && e.Version == expectedEvent.Version);

            Assert.Equal(expectedEvent.GetType(), actualEvent.GetType());
            Assert.Equal(JsonConvert.SerializeObject(expectedEvent), JsonConvert.SerializeObject(actualEvent));
        }
    }

    [Fact]
    public async Task CanSaveMemento()
    {
        var mementoStore = _serviceProvider.GetRequiredService<IMementoStore>();

        var expectedMemento = new PostMemento(Guid.NewGuid(), 10, "test", "test", Guid.NewGuid());

        await mementoStore.SaveAsync(expectedMemento);

        var actualMemento = await mementoStore.GetLatestMementoAsync(expectedMemento.SourcedId);

        Assert.Equal(JsonConvert.SerializeObject(expectedMemento), JsonConvert.SerializeObject(actualMemento));
    }
}