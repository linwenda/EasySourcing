namespace EasySourcing.Abstraction;

public interface IProjectorHandler<in T> where T : IVersionedEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken);
}