namespace EasySourcing.Abstraction;

public interface IEventHandler<in T> where T : IVersionedEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken);
}