using System.Reflection;
using EasySourcing.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EasySourcing;

public class Projector : IProjector
{
    private readonly IServiceProvider _serviceProvider;

    public Projector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ProjectAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IVersionedEvent
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var eventType = @event.GetType();
        var projectorHandlerType = typeof(IProjectorHandler<>).MakeGenericType(eventType);
        var projectorHandlers = scope.ServiceProvider.GetServices(projectorHandlerType);

        foreach (var projectorHandler in projectorHandlers)
        {
            var result = projectorHandlerType.GetTypeInfo()
                .GetDeclaredMethod(nameof(IProjectorHandler<TEvent>.HandleAsync))
                ?.Invoke(projectorHandler, new object[] {@event, cancellationToken});

            await ((Task) result)!;
        }
    }
}