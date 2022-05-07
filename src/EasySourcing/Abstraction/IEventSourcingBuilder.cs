using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing.Abstraction;

public interface IEventSourcingBuilder
{
    IServiceCollection Services { get; }
}