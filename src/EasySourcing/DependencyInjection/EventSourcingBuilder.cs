using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.DependencyInjection;

public class EventSourcingBuilder : IEventSourcingBuilder
{
    public EventSourcingBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}