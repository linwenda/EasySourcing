using System.Reflection;
using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public EventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IVersionedEvent
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var eventType = @event.GetType();
        var projectorHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var projectorHandlers = scope.ServiceProvider.GetServices(projectorHandlerType);

        foreach (var projectorHandler in projectorHandlers)
        {
            var result = projectorHandlerType.GetTypeInfo()
                .GetDeclaredMethod(nameof(IEventHandler<TEvent>.HandleAsync))
                ?.Invoke(projectorHandler, new object[] {@event, cancellationToken});

            await ((Task) result)!;
        }
    }
}