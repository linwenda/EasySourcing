using System.Collections.Generic;
using System.Linq;
using EasySourcing.Abstraction;
using Xunit;

namespace EasySourcing.UnitTests;

public static class EventSourcedTestExtensions
{
    public static TEvent Have<TEvent>(this IEnumerable<IVersionedEvent> versionedEvents) where TEvent : IVersionedEvent
    {
        var domainEvent = versionedEvents.FirstOrDefault(e => e.GetType() == typeof(TEvent));

        Assert.NotNull(domainEvent);

        return (TEvent)domainEvent;
    }
}