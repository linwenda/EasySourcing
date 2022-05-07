namespace EasySourcing.Abstraction;

public interface IProjector
{
    Task ProjectAsync<TEvent>(TEvent events, CancellationToken cancellationToken = default)
        where TEvent : IVersionedEvent;
}