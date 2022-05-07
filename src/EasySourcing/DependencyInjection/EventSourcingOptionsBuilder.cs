using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.DependencyInjection;

public class EventSourcingOptionsBuilder : IEventSourcingBuilder
{
    public EventSourcingOptionsBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
    public int TakeSnapshotEveryVersion { get; set; } = 10;
}